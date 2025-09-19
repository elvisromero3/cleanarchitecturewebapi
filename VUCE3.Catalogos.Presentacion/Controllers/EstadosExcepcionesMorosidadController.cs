using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class EstadosExcepcionesMorosidadController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public EstadosExcepcionesMorosidadController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerEstadosExcepcionesMorosidadQuery();
            var result = await _mediator.Send(query);
            return result.Match(
                estados => Ok(_mapper.Map<List<EstadoExcepcionMorosidadDto>>(estados)),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerEstadosExcepcionesMorosidadPorIdQuery() { Id = key };
            var result = await _mediator.Send(query);

            return result.Match(
                estado => Ok(_mapper.Map<EstadoExcepcionMorosidadDto>(estado)),
                errors => Problem(errors));
        }
    }
}
