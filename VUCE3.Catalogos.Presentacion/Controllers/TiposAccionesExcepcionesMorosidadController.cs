using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{   
    public class TiposAccionesExcepcionesMorosidadController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public TiposAccionesExcepcionesMorosidadController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerTiposAccionesExcepcionesMorosidadQuery();
            var result = await _mediator.Send(query);
            return result.Match(
                tipos => Ok(_mapper.Map<List<TipoAccionExcepcionMorosidadDto>>(tipos)),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery() { Id = key };
            var result = await _mediator.Send(query);

            return result.Match(
                tipo => Ok(_mapper.Map<TipoAccionExcepcionMorosidadDto>(tipo)),
                errors => Problem(errors));
        }
    }
}
