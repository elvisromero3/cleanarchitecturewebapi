using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.CrearSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EditarSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancias;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustanciaPorId;
using VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustancias;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class SustanciaController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        public SustanciaController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerSustanciasQuery();
            var result = await _mediator.Send(query);
            return result.Match(
                        sustancias => Ok(_mapper.Map<List<SustanciaDto>>(sustancias)),
                        errors => Problem(errors));
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage))
                    .ToList();
                return Problem(errors, false);
            }
            var query = new ObtenerSustanciaPorIdQuery()
            {
                IdSustancia = key
            };
            var result = await _mediator.Send(query);
            return result.Match(
                        sustancia => Ok(_mapper.Map<SustanciaDto>(sustancia)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] SustanciaDto sustanciaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var command = new CrearSustanciaCommand()
            {
                Sustancia = _mapper.Map<Sustancia>(sustanciaDto)
            };
            var result = await _mediator.Send(command);
            return result.Match(
                        sustancia => Created(_mapper.Map<SustanciaDto>(sustancia)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<SustanciaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage))
                    .ToList();
                return Problem(errors, false);
            }

            var resultDTO = new SustanciaDto();
            deltaDto.Patch(resultDTO);
            var sustancia = _mapper.Map<Sustancia>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarSustanciaCommand()
            {
                IdSustancia = key,
                Sustancia = sustancia,
                ListaCambios = changeList
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        updated => Ok(new Tuple<SustanciaDto, SustanciaDto>(_mapper.Map<SustanciaDto>(updated.Item1),
                        _mapper.Map<SustanciaDto>(updated.Item2))),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage))
                    .ToList();
                return Problem(errors, false);
            }

            var command = new EliminarSustanciaCommand()
            {
                IdSustancia = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<SustanciaDto>(deleted)),
                errors => Problem(errors));
        }

        [HttpPost]
        public async Task<IActionResult> BorradoMasivo(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var items = param["items"] as IEnumerable<int>;

            var command = new EliminarSustanciasCommand()
            {
                IdsSustancias = items ?? []
            };

            var resultado = await _mediator.Send(command);

            return resultado.Match(
                deleted => NoContent(),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(z => z.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(error, false);
            }

            var temp = (IEnumerable<ImportarSustanciaDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarSustanciaDto> datos;
            try
            {
                datos = temp.ToList();
            }
            catch (Exception)
            {
                return EstructuraArchivoImportacionInvalido();
            }

            var camposInvalidos = ValidadorDto.IsAnyNullOrEmpty(datos);

            if (camposInvalidos)
            {
                return EstructuraArchivoImportacionInvalido();
            }

            var command = new ImportarDatosCommand()
            {
                Modo = modo,
                Datos = datos.Select(x => new Sustancia()
                {
                   Nombre = x.Nombre,
                   Cas = x.Cas,
                   ListaCaq = x.ListaCaq
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
