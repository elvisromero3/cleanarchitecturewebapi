using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVucePorId;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.CrearNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EditarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.ImportarDatos;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarMasivoNoticiasVuce;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class NoticiasVuceController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        
        public NoticiasVuceController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {

            var query = new ObtenerNoticiasVuceQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                noticiasVuce => Ok(_mapper.Map<List<NoticiasVuceDto>>(noticiasVuce)),
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

            var query = new ObtenerNoticiasVucePorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<NoticiasVuceDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] NoticiasVuceDto noticiasVuceDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearNoticiasVuceCommand()
            {
                NoticiasVuce = _mapper.Map<NoticiasVuce>(noticiasVuceDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        resultNoticiasVuce => Created(_mapper.Map<NoticiasVuceDto>(resultNoticiasVuce)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<NoticiasVuceDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new NoticiasVuceDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<NoticiasVuce>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarNoticiasVuceCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<NoticiasVuceDto, NoticiasVuceDto>(_mapper.Map<NoticiasVuceDto>(updated.Item1),
                _mapper.Map<NoticiasVuceDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var command = new EliminarNoticiasVuceCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<NoticiasVuceDto>(deleted)),
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
            var temp = (IEnumerable<ImportarNoticiasVuceDto>)param["datos"];
            List<ImportarNoticiasVuceDto> datos;
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
                Datos = _mapper.Map<IEnumerable<NoticiasVuce>>(datos)
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

            var command = new EliminarMasivoNoticiasVuceCommand()
            {
                IdsNoticias = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
