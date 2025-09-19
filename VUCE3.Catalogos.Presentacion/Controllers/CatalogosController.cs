using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos;
using VUCE3.Catalogos.Presentacion.DTO;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogosPorIdInstitucion;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogoPorId;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class CatalogosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public CatalogosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerCatalogosQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        catalogos => Ok(_mapper.Map<List<CatalogoDto>>(catalogos)),
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

            var query = new ObtenerCatalogoPorIdQuery()
            {
                IdCatalogo = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<CatalogoDto>(result)),
              errors => Problem(errors));
        }

        [HttpGet("odata/Catalogos/ObtenerCatalogosPorInstitucion")]
        [EnableQuery]
        public async Task<IActionResult> ObtenerCatalogosPorInstitucion(int idInstitucion)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerCatalogosPorIdInstitucionQuery
            {
                IdInstitucion = idInstitucion
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        catalogos => Ok(_mapper.Map<List<CatalogoDto>>(catalogos)),
                        errors => Problem(errors));
        }

    }
}
