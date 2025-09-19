using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class TarifasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public TarifasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerTarifasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        tarifas => Ok(_mapper.Map<List<TarifaDto>>(tarifas)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerTarifaPorIdQuery()
            {
                IdTarifa = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        tarifa => Ok(_mapper.Map<TarifaDto>(tarifa)),
                        errors => Problem(errors));
        }
    }
}
