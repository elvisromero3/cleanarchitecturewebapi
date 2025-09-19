using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using System.Globalization;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.CrearExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EditarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionMorosidadPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class ExcepcionMorosidadController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ExcepcionMorosidadController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerExcepcionesMorosidadQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        excepcion => Ok(_mapper.Map<List<ExcepcionMorosidadDto>>(excepcion)),
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

            var query = new ObtenerExcepcionMorosidadPorIdQuery()
            {
                IdExcepcionMorosidad = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<ExcepcionMorosidadDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] ExcepcionMorosidadDto excepcionMorosidadDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearExcepcionMorosidadCommand()
            {
                ExcepcionMorosidad = _mapper.Map<ExcepcionMorosidad>(excepcionMorosidadDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<ExcepcionMorosidadDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ExcepcionMorosidadDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new ExcepcionMorosidadDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<ExcepcionMorosidad>(resultDto);

            var command = new EditarExcepcionMorosidadCommand()
            {
                IdExcepcionMorosidad = key,
                ExcepcionMorosidad = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<ExcepcionMorosidadDto, ExcepcionMorosidadDto>(_mapper.Map<ExcepcionMorosidadDto>(updated.Item1),
                _mapper.Map<ExcepcionMorosidadDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarExcepcionMorosidadCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ExcepcionMorosidadDto>(deleted)),
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

            var command = new EliminarExcepcionesMorosidadCommand()
            {
                IdsExcepcionesMorosidad = items ?? []
            };

            var res = await _mediator.Send(command);

            return res.Match(
                deleted => NoContent(),
                error => Problem(error));
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDatos(ODataActionParameters param)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(error, false);
            }

            var modoImportar = (int)param["modo"];

            IEnumerable<ImportarExcepcionMorosidadDto> temp = (IEnumerable<ImportarExcepcionMorosidadDto>)param["datos"];

            List<ImportarExcepcionMorosidadDto> datos;
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

            foreach (var dato in datos)
            {
                if (!string.IsNullOrWhiteSpace(dato.FechaVencimiento) 
                    && !DateTime.TryParseExact(
                        dato.FechaVencimiento,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime _)                    
                    )
                {
                    return Problem([ErroresExcepcionMorosidad.FechaVencimientoInvalida]);
                }

                if (!DateTime.TryParseExact(
                        dato.FechaInicio,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime _)                   
                    )
                {
                    return Problem([ErroresExcepcionMorosidad.FechaInicioInvalida]);
                }

            }

            var datosImportar = datos.Select(x =>
            {
                DateTime.TryParse(x.FechaVencimiento, out var fechaV);
                DateTime.TryParse(x.FechaInicio, out var fechaI);

                return new ExcepcionMorosidad()
                {
                    IdTipoTramite = x.Tramite,
                    IdSubtipoTramite = x.TipoTramite,
                    IdRegimen = x.Regimen,
                    IdTipoAccion = x.TipoAccion,
                    FechaInicio = fechaI.Date,
                    FechaVencimiento = string.IsNullOrWhiteSpace(x.FechaVencimiento) ? null : fechaV.Date,
                    TipoIdentificacionEmpresa = x.TipoIdentificacion,
                    NumeroIdentificacionEmpresa = x.NumeroIdentificacion,
                    NombreEmpresa = x.Empresa,
                    Observaciones = x.Observaciones
                };
            });

            var command = new ImportarDatosCommand()
            {
                Modo = modoImportar,
                Datos = datosImportar                
            };

            var results = await _mediator.Send(command);

            return results.Match(
                deleted => Created(),
                error => Problem(error));
        }
    }
}
