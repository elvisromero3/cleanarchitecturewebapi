using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EditarBloqueComercial;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.CrearBloqueComercial;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EliminarBloqueComercial;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.EliminarBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.Importardatos;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class BloquesComercialesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public BloquesComercialesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerBloquesComercialesQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        provincias => Ok(_mapper.Map<List<BloqueComercialDto>>(provincias)),
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

            var query = new ObtenerBloqueComercialPorIdQuery()
            {
              IdBloqueComercial = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<BloqueComercialDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] BloqueComercialDto provinciaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearBloqueComercialCommand()
            {
                BloqueComercial = _mapper.Map<BloqueComercial>(provinciaDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<BloqueComercialDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<BloqueComercialDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new BloqueComercialDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<BloqueComercial>(resultDto);

            var command = new EditarBloqueComercialCommand()
            {
               IdBloqueComercial = key,
                BloqueComercial = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<BloqueComercialDto, BloqueComercialDto>(_mapper.Map<BloqueComercialDto>(updated.Item1),
                _mapper.Map<BloqueComercialDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarBloqueComercialCommand()
            {
              Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<BloqueComercialDto>(deleted)),
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

            var command = new EliminarBloquesComercialesCommand()
            {
                IdsBloquesComerciales = items ?? []
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
                        
            var temp = (IEnumerable<ImportarBloqueComercialDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarBloqueComercialDto> datos;
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
                Datos = datos.Select(x => new BloqueComercial()
                {
                    Nombre = x.Nombre
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }

    }
}
