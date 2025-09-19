using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.CrearPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EditarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.ImportarDatos;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPaises;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using VUCE3.Catalogos.Dominio.Errores;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class PaisesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public PaisesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        [EnableQuery]
        public async Task<IActionResult> Get()
        {           

            var query = new ObtenerPaisesQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                paises => Ok(_mapper.Map<List<PaisDto>>(paises)),
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

            var query = new ObtenerPaisPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<PaisDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] PaisDto paisDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearPaisCommand()
            {
                Pais = _mapper.Map<Pais>(paisDto)
            };

            var result = await _mediator.Send(command);            

            return result.Match(
                        resultPais => Created(_mapper.Map<PaisDto>(resultPais)),
                        errors => Problem(errors));
        }
        
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<PaisDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new PaisDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Pais>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarPaisCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<PaisDto, PaisDto>(_mapper.Map<PaisDto>(updated.Item1),
                _mapper.Map<PaisDto>(updated.Item2))),
                errors => Problem(errors));
        }
        
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarPaisCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<PaisDto>(deleted)),
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
            var temp = (IEnumerable<ImportarPaisDto>)param["datos"];            

            List<ImportarPaisDto> datos;
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
                Datos = _mapper.Map<IEnumerable<Pais>>(datos)
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

            var command = new EliminarPaisesCommand()
            {
                IdsPaises = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
