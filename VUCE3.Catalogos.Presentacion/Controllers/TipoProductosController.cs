using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductos;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EditarTipoProducto;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductoPorId;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.CrearTipoProducto;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EliminarTipoProducto;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class TipoProductosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public TipoProductosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerTipoProductosQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        provincias => Ok(_mapper.Map<List<TipoProductoDto>>(provincias)),
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

            var query = new ObtenerTipoProductoPorIdQuery()
            {
              IdTipoProducto = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<TipoProductoDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] TipoProductoDto provinciaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearTipoProductoCommand()
            {
                TipoProducto = _mapper.Map<TipoProducto>(provinciaDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<TipoProductoDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<TipoProductoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new TipoProductoDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<TipoProducto>(resultDto);

            var command = new EditarTipoProductoCommand()
            {
               IdTipoProducto = key,
                TipoProducto = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<TipoProductoDto, TipoProductoDto>(_mapper.Map<TipoProductoDto>(updated.Item1),
                _mapper.Map<TipoProductoDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarTipoProductoCommand()
            {
              Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<TipoProductoDto>(deleted)),
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

            IEnumerable<ImportarTipoProductoDto> temp = (IEnumerable<ImportarTipoProductoDto>)param["datos"];

            List<ImportarTipoProductoDto> datos;
            try
            {
                datos = temp.ToList();
            }
            catch (Exception)
            {
                return EstructuraArchivoImportacionInvalido();
            }


            var command = new ImportarDatosCommand
            {
                Modo = modo,
                Datos = _mapper.Map<List<ImportarTipoProductoCommandDto>>(datos)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
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

            var command = new EliminarTipoProductosCommand()
            {
                IdsTipoProductos = items ?? []
            };

            var resultImportarTipoProductos = await _mediator.Send(command);

            return resultImportarTipoProductos.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }


    }
}
