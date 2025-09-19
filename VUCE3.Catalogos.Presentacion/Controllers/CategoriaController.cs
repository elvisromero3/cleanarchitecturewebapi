using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.CrearCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EditarCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategorias;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategorias;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    public class CategoriaController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public CategoriaController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerCategoriasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                        casas => Ok(_mapper.Map<List<CategoriaDto>>(casas)),
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

            var query = new ObtenerCategoriaPorIdQuery()
            {
                IdCategoria = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<CategoriaDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] CategoriaDto CategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearCategoriaCommand()
            {
                Categoria = _mapper.Map<Categoria>(CategoriaDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<CategoriaDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<CategoriaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new CategoriaDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Categoria>(resultDto);

            var command = new EditarCategoriaCommand()
            {
                IdCategoria = key,
                Categoria = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<CategoriaDto, CategoriaDto>(_mapper.Map<CategoriaDto>(updated.Item1),
                _mapper.Map<CategoriaDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarCategoriaCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<CategoriaDto>(deleted)),
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

            var command = new EliminarCategoriasCommand()
            {
                IdsCategorias = items ?? []
            };

            var resultCategoria = await _mediator.Send(command);

            return resultCategoria.Match(
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

            IEnumerable<ImportarCategoriaDto> temp = (IEnumerable<ImportarCategoriaDto>)param["datos"];

            List<ImportarCategoriaDto> datos;
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
                Datos = _mapper.Map<IEnumerable<ImportarCategoriaCommandDto>>(datos)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
