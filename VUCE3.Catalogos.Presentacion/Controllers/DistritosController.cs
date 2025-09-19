using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.CrearDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EditarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritoPorId;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;

namespace VUCE3.Catalogos.Presentacion.Controllers
{
    
    public class DistritosController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public DistritosController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerDistritosQuery();
            
            var result = await _mediator.Send(query);

            return result.Match(
                        distritos => Ok(_mapper.Map<List<DistritoDto>>(distritos)),
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

            var query = new ObtenerDistritoPorIdQuery
            {
                Id= key
            };
            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<DistritoDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] DistritoDto distritoDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearDistritoCommand()
            {
                Distrito = _mapper.Map<Distrito>(distritoDto)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<DistritoDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<DistritoDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            var resultDto = new DistritoDto();
            deltaDto.Patch(resultDto);
            var listaCambios = deltaDto.GetChangedPropertyNames().ToList();
            var result = _mapper.Map<Distrito>(resultDto);

            var command = new EditarDistritoCommand()
            {
                IdDistrito = key,
                Distrito = result,
                ListaCambios = listaCambios
            };

            var resultUpdate = await _mediator.Send(command);

            return resultUpdate.Match(
                updated => Ok(new Tuple<DistritoDto, DistritoDto>(_mapper.Map<DistritoDto>(updated.Item1),
                _mapper.Map<DistritoDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarDistritoCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<DistritoDto>(deleted)),
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

            var command = new EliminarDistritosCommand()
            {
                IdsDistritos = items ?? []
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
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => Error.Validation(description: y.Exception?.Message ?? y.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var temp = (IEnumerable<ImportarDistritoDto>)param["datos"];
            var modo = (int)param["modo"];

            List<ImportarDistritoDto> datos;
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
                Datos = datos.Select(x => new ImportarDistritoCommandDto()
                {
                    Provincia = x.Provincia,
                    Canton = x.Canton,
                    Codigo = x.CodigoDistrito,
                    Nombre = x.Distrito
                })
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Created(),
                errors => Problem(errors));
        }
    }
}
