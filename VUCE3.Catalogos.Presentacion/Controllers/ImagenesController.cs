using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenesPorId;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.CrearImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EditarImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarMasivoImagenes;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class ImagenesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ImagenesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        [EnableQuery]
        public async Task<IActionResult> Get()
        {           

            var query = new ObtenerImagenesQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                Imagenes => Ok(_mapper.Map<List<ImagenesDto>>(Imagenes)),
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

            var query = new ObtenerImagenesPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<ImagenesDto>(result)),
              errors => Problem(errors));
        }        

        public async Task<IActionResult> Post([FromBody] ImagenesDto imagenesDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearImagenesCommand()
            {
                Imagenes = _mapper.Map<Imagenes>(imagenesDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        resultImagen => Created(_mapper.Map<ImagenesDto>(resultImagen)),
                        errors => Problem(errors));
        }
        
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ImagenesDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new ImagenesDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Imagenes>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarImagenesCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<ImagenesDto, ImagenesDto>(_mapper.Map<ImagenesDto>(updated.Item1),
                _mapper.Map<ImagenesDto>(updated.Item2))),
                errors => Problem(errors));
        }
        
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarImagenesCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ImagenesDto>(deleted)),
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

            var command = new EliminarMasivoImagenesCommand()
            {
                IdsImagenes = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
