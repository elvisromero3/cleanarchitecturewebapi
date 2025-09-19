using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresaPorId;
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.CrearEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EditarEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{    
    public class EmpresasController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public EmpresasController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        [EnableQuery(MaxNodeCount =200)]
        public async Task<IActionResult> Get()
        {               

            var query = new ObtenerEmpresasQuery();

            var result = await _mediator.Send(query);

            return result.Match(
                empresas => Ok(_mapper.Map<List<EmpresaDto>>(empresas)),
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

            var query = new ObtenerEmpresaPorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
              result => Ok(_mapper.Map<EmpresaDto>(result)),
              errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] EmpresaDto empresaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new CrearEmpresaCommand()
            {
                Empresa = _mapper.Map<Empresa>(empresaDto)
            };

            var result = await _mediator.Send(command);            

            return result.Match(
                        resultEmpresa => Created(_mapper.Map<EmpresaDto>(resultEmpresa)),
                        errors => Problem(errors));
        }
        
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<EmpresaDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var resultDTO = new EmpresaDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Empresa>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();

            var command = new EditarEmpresaCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<EmpresaDto, EmpresaDto>(_mapper.Map<EmpresaDto>(updated.Item1),
                _mapper.Map<EmpresaDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarEmpresaCommand()
            {
                Id = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<EmpresaDto>(deleted)),
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
            IEnumerable<ImportarEmpresaDto> temp =(IEnumerable<ImportarEmpresaDto>)param["datos"];
            List<ImportarEmpresaDto> datos;
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
                Datos = datos.Select(x => new ImportarEmpresaCommandDto()
                {
                   IdProfesional = x.IdProfesional,
                   Nombre = x.Nombre,
                   NumeroIdentificacion = x.NumeroIdentificacion,
                   TipoIdentificacion = x.TipoIdentificacion
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

            var command = new EliminarEmpresasCommand()
            {
               IdsEmpresas  = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
