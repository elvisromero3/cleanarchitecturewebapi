using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvincias;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EditarProvincia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvinciaPorId;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.CrearProvincia;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EliminarProvincia;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using ErrorOr;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.EliminarProvincias;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.ImportarDatos;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class ProvinciasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ProvinciasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerProvinciasQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        provincias => Ok(_mapper.Map<List<ProvinciaDto>>(provincias)),
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

            var query = new ObtenerProvinciaPorIdQuery()
            {
              IdProvincia = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<ProvinciaDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] ProvinciaDto provinciaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearProvinciaCommand()
            {
                Provincia = _mapper.Map<Provincia>(provinciaDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<ProvinciaDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ProvinciaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new ProvinciaDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Provincia>(resultDto);

            var command = new EditarProvinciaCommand()
            {
               IdProvincia = key,
                Provincia = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<ProvinciaDto, ProvinciaDto>(_mapper.Map<ProvinciaDto>(updated.Item1),
                _mapper.Map<ProvinciaDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarProvinciaCommand()
            {
              Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ProvinciaDto>(deleted)),
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

            var command = new EliminarProvinciasCommand()
            {
                IdsProvincias = items ?? []
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
                var error = ModelState.Values.SelectMany(y => y.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(error, false);
            }

            var temp = (IEnumerable<ImportarProvinciaDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarProvinciaDto> datos;
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
                Datos = datos.Select(x => new Provincia()
                {
                    Codigo = x.CodigoProvincia,
                    Nombre = x.Provincia
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
