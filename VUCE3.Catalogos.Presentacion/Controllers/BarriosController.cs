using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.CrearBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EditarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrios;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrioPorId;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class BarriosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public BarriosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerBarriosQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        barrios => Ok(_mapper.Map<List<BarrioDto>>(barrios)),
                        errors => Problem(errors));
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerBarrioPorIdQuery()
            {
                Id = key
            };
            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<BarrioDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] BarrioDto barrioDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearBarrioCommand()
            {
                Barrio = _mapper.Map<Barrio>(barrioDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<BarrioDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<BarrioDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new BarrioDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Barrio>(resultDto);

            var command = new EditarBarrioCommand()
            {
                IdBarrio = key,
                Barrio = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<BarrioDto, BarrioDto>(_mapper.Map<BarrioDto>(updated.Item1),
                _mapper.Map<BarrioDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarBarrioCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<BarrioDto>(deleted)),
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

            var command = new EliminarBarriosCommand()
            {
                IdsBarrios = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(y => y.Errors).Select(y => Error.Validation(description: y.Exception?.Message ?? y.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var temp = (IEnumerable<ImportarBarrioDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarBarrioDto> datos;
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
                Datos = datos.Select(x => new ImportarBarrioCommandDto()
                {
                    Provincia = x.Provincia,
                    Canton = x.Canton,
                    Distrito = x.Distrito,
                    Codigo = x.CodigoBarrio,
                    Nombre = x.Barrio
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
