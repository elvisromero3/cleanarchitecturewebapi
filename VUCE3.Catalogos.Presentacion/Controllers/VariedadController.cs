using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EditarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedadPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class VariedadController : ODataControllerBase
    {        
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public VariedadController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerVariedadesQuery();
            var result = await _mediator.Send(query);

            return result.Match(
                variedades => Ok(_mapper.Map<List<VariedadDto>>(variedades)),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerVariedadPorIdQuery { Id = key };
            var result = await _mediator.Send(query);

            return result.Match(
                variedad => Ok(_mapper.Map<VariedadDto>(variedad)),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] VariedadDto variedadDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearVariedadCommand()
            {
                Variedad = _mapper.Map<Variedad>(variedadDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<VariedadDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<VariedadDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new VariedadDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Variedad>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarVariedadCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<VariedadDto, VariedadDto>(_mapper.Map<VariedadDto>(updated.Item1),
                _mapper.Map<VariedadDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarVariedadCommand { IdVariedad = key };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<VariedadDto>(deleted)),
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
            var temp = (IEnumerable<ImportarVariedadDto>)param["datos"];
            List<ImportarVariedadDto> datos;
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
                Datos = _mapper.Map<IEnumerable<Variedad>>(datos)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
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

            var command = new EliminarVariedadesCommand()
            {
                IdsVariedades = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
