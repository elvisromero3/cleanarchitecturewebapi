using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.CrearRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EditarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitoPorId;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;

using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class RequisitosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public RequisitosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerRequisitosQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        requisitos => Ok(_mapper.Map<List<RequisitoDto>>(requisitos)),
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

            var query = new ObtenerRequisitoPorIdQuery
            {
                Id = key
            };
            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<RequisitoDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] RequisitoDto requisitoDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearRequisitoCommand()
            {
                Requisito = _mapper.Map<Requisito>(requisitoDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<RequisitoDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<RequisitoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new RequisitoDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Requisito>(resultDto);

            var command = new EditarRequisitoCommand()
            {
                IdRequisito = key,
                Requisito = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<RequisitoDto, RequisitoDto>(_mapper.Map<RequisitoDto>(updated.Item1),
                _mapper.Map<RequisitoDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarRequisitoCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<RequisitoDto>(deleted)),
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

            IEnumerable<ImportarRequisitoDto> temp = (IEnumerable<ImportarRequisitoDto>)param["datos"];

            List<ImportarRequisitoDto> datos;
            try
            {
                datos = temp.ToList();
            }
            catch (Exception)
            {
                return EstructuraArchivoImportacionInvalido();
            }

   
            var command = new ImportarDatosCommand
            {
                Modo = modo,
                Datos = _mapper.Map<List<ImportarRequisitoCommandDto>>(datos)
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

            var command = new EliminarRequisitosCommand()
            {
               IdsRequisitos = items ?? []
            };

            var resultImportarFamilia = await _mediator.Send(command);

            return resultImportarFamilia.Match(
                deleted => NoContent(),
                errors => Problem(errors));

        }
    }
}
