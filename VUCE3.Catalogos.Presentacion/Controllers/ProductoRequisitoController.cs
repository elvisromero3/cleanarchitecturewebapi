using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitosPorId;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.CrearProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisitos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;


namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class ProductoRequisitosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ProductoRequisitosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerProductoRequisitoQuery()
            {
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        familias => Ok(_mapper.Map<List<ProductoRequisitoDto>>(familias)),
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

            var query = new ObtenerProductoRequisitoPorIdQuery()
            {
                IdProductoRequisito = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<ProductoRequisitoDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] ProductoRequisitoDto familiaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearProductoRequisitoCommand()
            {
                ProductoRequisito = _mapper.Map<ProductoRequisito>(familiaDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<ProductoRequisitoDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ProductoRequisitoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new ProductoRequisitoDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<ProductoRequisito>(resultDto);

            var command = new EditarProductoRequisitoCommand()
            {
                IdProductoRequisito = key,
                ProductoRequisito = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<ProductoRequisitoDto, ProductoRequisitoDto>(_mapper.Map<ProductoRequisitoDto>(updated.Item1),
                _mapper.Map<ProductoRequisitoDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarProductoRequisitoCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ProductoRequisitoDto>(deleted)),
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

            IEnumerable<ImportarProductoRequisitoDto> temp = (IEnumerable<ImportarProductoRequisitoDto>)param["datos"];

            List<ImportarProductoRequisitoDto> datos;
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
                Datos = _mapper.Map<List<ImportarProductoRequisitoCommandDto>>(datos)
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

            var command = new EliminarProductoRequisitosCommand()
            {
                IdsRequisitos = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));

        }
    }
}
