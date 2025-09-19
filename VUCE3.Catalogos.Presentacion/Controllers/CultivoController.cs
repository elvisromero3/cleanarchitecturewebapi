using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.CrearCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EditarCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivos;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos.DTO;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class CultivoController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public CultivoController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerCultivosQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        cultivos => Ok(_mapper.Map<List<CultivoDto>>(cultivos)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerCultivoPorIdQuery()
            {
                IdCultivo = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        cultivo => Ok(_mapper.Map<CultivoDto>(cultivo)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] CultivoDto cultivoDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearCultivoCommand()
            {
                Cultivo = _mapper.Map<Cultivo>(cultivoDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                resultCultivo => Created(_mapper.Map<CultivoDto>(resultCultivo)),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<CultivoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new CultivoDto();
            deltaDto.Patch(resultDTO);
            var cultivo = _mapper.Map<Cultivo>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarCultivoCommand()
            {
                IdCultivo = key,
                Cultivo = cultivo,
                ListaCambios = changeList
            };

            var result = await _mediator.Send(command);

            return result.Match(
                updated => Ok(new Tuple<CultivoDto, CultivoDto>(_mapper.Map<CultivoDto>(updated.Item1),
                _mapper.Map<CultivoDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarCultivoCommand()
            {
                IdCultivo = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<CultivoDto>(deleted)),
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

            var command = new EliminarCultivosCommand()
            {
                IdsCultivos = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));

        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var modo = (int)param["modo"];
            var temp = (IEnumerable<ImportarCultivoDto>)param["datos"];
            List<ImportarCultivoDto> datos;
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
                Datos = datos.Select(x => new ImportarCultivoCommandDto()
                {
                    Nombre = x.Nombre,
                    Codigo = x.Codigo,
                    CodigoVariedad = x.CodigoVariedad,
                    NombreCientifico = x.NombreCientifico
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
