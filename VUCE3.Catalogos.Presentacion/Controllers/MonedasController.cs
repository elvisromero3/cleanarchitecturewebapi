using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedaPorId;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class MonedasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public MonedasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerMonedasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        monedas => Ok(_mapper.Map<List<MonedaDto>>(monedas)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerMonedaPorIdQuery()
            {
                IdMoneda = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        moneda => Ok(_mapper.Map<MonedaDto>(moneda)),
                        errors => Problem(errors));
        }
    }
}
