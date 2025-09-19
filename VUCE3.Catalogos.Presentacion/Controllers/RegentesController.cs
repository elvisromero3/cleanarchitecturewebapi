using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionalPorId;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.CrearProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EditarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;


namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class RegentesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        
        public RegentesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        [EnableQuery]
        public async Task<IActionResult> Get()
        {           

            var query = new ObtenerProfesionalesQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                profesionales => Ok(_mapper.Map<List<ProfesionalDto>>(profesionales)),
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

            var query = new ObtenerProfesionalPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<ProfesionalDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] ProfesionalDto profesionalDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            
            var command = new CrearProfesionalCommand()
            {
                Profesional = _mapper.Map<Profesional>(profesionalDto)
            };

            var result = await _mediator.Send(command);            

            return result.Match(
                        resultProfesional => Created(_mapper.Map<ProfesionalDto>(resultProfesional)),
                        errors => Problem(errors));
        }
        
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ProfesionalDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new ProfesionalDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Profesional>(resultDTO);

            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarProfesionalCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<ProfesionalDto, ProfesionalDto>(_mapper.Map<ProfesionalDto>(updated.Item1),
                _mapper.Map<ProfesionalDto>(updated.Item2))),
                errors => Problem(errors));
        }
        
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarProfesionalCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ProfesionalDto>(deleted)),
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
            var temp = (IEnumerable<ImportarProfesionalDto>)param["datos"];
            List<ImportarProfesionalDto> datos;
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
                Datos = datos.Select(x => new ImportarProfesionalCommandDto()
                {
                    Nombre = x.Nombre,
                    TipoIdentificacion = x.TipoIdentificacion,
                    Activo = x.Activo,
                    CodigoRegente = x.CodigoRegente,
                    Email = x.Email,
                    Profesion = x.Profesion,
                    NumeroIdentificacion =  x.NumeroIdentificacion,
                    Institucion = x.Institucion,
                    IdInstitucion = x.IdInstitucion,
                })
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

            var command = new EliminarProfesionalesCommand()
            {
                IdsProfesionales = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
