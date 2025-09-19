using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.CrearCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EditarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicaPorId;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristicas;

namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class CaracteristicasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public CaracteristicasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerCaracteristicasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        caracteristicas => Ok(_mapper.Map<List<CaracteristicaDto>>(caracteristicas)),
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

            var query = new ObtenerCaracteristicaPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<CaracteristicaDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] CaracteristicaDto caracteristica)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearCaracteristicaCommand()
            {
                Caracteristica = _mapper.Map<Caracteristica>(caracteristica)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        resultCaracteristica => Created(_mapper.Map<CaracteristicaDto>(resultCaracteristica)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<CaracteristicaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new CaracteristicaDto();
            deltaDto.Patch(resultDTO);
            var caracteristica = _mapper.Map<Caracteristica>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarCaracteristicaCommand(caracteristica, key, changeList);

            var result = await _mediator.Send(command);

            return result.Match(
                updated => Ok(new Tuple<CaracteristicaDto, CaracteristicaDto>(_mapper.Map<CaracteristicaDto>(updated.Item1),
                _mapper.Map<CaracteristicaDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarCaracteristicaCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<CaracteristicaDto>(deleted)),
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

            int modo = (int)param["modo"];
            IEnumerable<ImportarCaracteristicaDto> temp = (IEnumerable<ImportarCaracteristicaDto>)param["datos"];

            List<ImportarCaracteristicaDto> datos;
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
                Datos = datos.Select(x => new ImportarDatosCaracteristicaDto()
                {
                    Institucion = x.Institucion ?? "",
                    Nombre = x.Nombre,
                    IdInstitucion = x.IdInstitucion
                })
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

            var ids = (IEnumerable<int>)param["items"];

            var command = new EliminarCaracteristicasCommand()
            {
                Ids = ids
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
