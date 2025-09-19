using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.CrearEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EditarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientoPorId;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Dominio.Errores;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class EstablecimientosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public EstablecimientosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new ObtenerEstablecimientosQuery());

            return result.Match(
                establecimientos => Ok(_mapper.Map<List<EstablecimientoDto>>(establecimientos)),
                errors => Problem(errors)
            );
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var result = await _mediator.Send(new ObtenerEstablecimientosPorIdQuery() { IdEstablecimiento = key });

            return result.Match(
                result => Ok(_mapper.Map<EstablecimientoDto>(result)),
                errors => Problem(errors)
            );
        }

        public async Task<IActionResult> Post([FromBody] EstablecimientoDto establecimientoDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var result = await _mediator.Send(new CrearEstablecimientoCommand() { Establecimiento = _mapper.Map<Dominio.Entidades.Establecimiento>(establecimientoDto) });

            return result.Match(
                establecimiento => Ok(_mapper.Map<EstablecimientoDto>(establecimiento)),
                errors => Problem(errors)
            );
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<EstablecimientoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var result = await _mediator.Send(new EditarEstablecimientoCommand()
            {
                IdEstablecimiento = key,
                Establecimiento = _mapper.Map<Dominio.Entidades.Establecimiento>(deltaDto.Patch(new EstablecimientoDto())),
                ListaCambios = deltaDto.GetChangedPropertyNames().ToList()
            });

            return result.Match(
                updated => Ok(new Tuple<EstablecimientoDto, EstablecimientoDto>(_mapper.Map<EstablecimientoDto>(updated.Item1), _mapper.Map<EstablecimientoDto>(updated.Item2))),
                errors => Problem(errors)
            );
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var result = await _mediator.Send(new EliminarEstablecimientoCommand() { Id = key });

            return result.Match(
                deleted => Ok(_mapper.Map<EstablecimientoDto>(deleted)),
                errors => Problem(errors)
            );
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

            var command = new EliminarEstablecimientosCommand()
            {
                IdsEstablecimientos = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
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
            var temp = (IEnumerable<ImportarEstablecimientoDto>)param["datos"];
            List<ImportarEstablecimientoDto> datos;
            try
            {
                datos = temp.ToList();
            }
            catch (Exception)
            {
                return EstructuraArchivoImportacionInvalido();
            }
            
            foreach (var fechaVencimiento in datos.Select(d=>d.FechaVencimiento))
            {
                if (fechaVencimiento is null || 
                    !DateTime.TryParseExact(
                        fechaVencimiento,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime _
                        )
                    )
                {
                    return Problem([ErroresEstablecimiento.FechaVencimientoMenorHoy]);
                }
            }

            var camposInvalidos = ValidadorDto.IsAnyNullOrEmpty(datos);
            if (camposInvalidos)
            {
                return EstructuraArchivoImportacionInvalido();
            }

            var command = new ImportarDatosCommand()
            {
                Modo = modo,
                Datos = _mapper.Map<IEnumerable<Establecimiento>>(datos)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
