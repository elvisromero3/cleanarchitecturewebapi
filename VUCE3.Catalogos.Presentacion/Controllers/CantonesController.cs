using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantones;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EditarCanton;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantonPorId;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.CrearCanton;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCanton;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using ErrorOr;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCantones;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos.DTO;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class CantonesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public CantonesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerCantonesQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        cantones => Ok(_mapper.Map<List<CantonDto>>(cantones)),
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

            var query = new ObtenerCantonPorIdQuery()
            {
              IdCanton = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<CantonDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] CantonDto cantonDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearCantonCommand()
            {
                Canton = _mapper.Map<Canton>(cantonDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<CantonDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<CantonDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new CantonDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Canton>(resultDto);

            var command = new EditarCantonCommand()
            {
               IdCanton = key,
                Canton = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<CantonDto, CantonDto>(_mapper.Map<CantonDto>(updated.Item1),
                _mapper.Map<CantonDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarCantonCommand()
            {
              Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<CantonDto>(deleted)),
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

            var command = new EliminarCantonesCommand()
            {
                IdsCantones = items ?? []
            };

            var results = await _mediator.Send(command);

            return results.Match(
                deleted => NoContent(),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(z => z.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var temp = (IEnumerable<ImportarCantonDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarCantonDto> datos;
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
                Datos = datos.Select(x => new ImportarCantonCommandDto()
                {
                    Provincia = x.Provincia,
                    Codigo = x.CodigoCanton,
                    Nombre = x.Canton
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
