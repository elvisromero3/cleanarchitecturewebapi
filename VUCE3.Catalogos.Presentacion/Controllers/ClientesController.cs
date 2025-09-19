using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.CrearCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EditarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarClientes;
using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientePorId;
using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientes;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.DTO;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Validadores;
using ErrorOr;

namespace VUCE3.Catalogos.Presentacion.Controllers
{   
    public class ClientesController : ODataControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ClientesController(ISender mediator, IMapper mapper, IStringLocalizer<ILocalization> localizer) : base(localizer)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var query = new ObtenerClientesQuery();
            var result = await _mediator.Send(query);

            return result.Match(
                        clientes => Ok(_mapper.Map<List<ClienteDto>>(clientes)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Get(int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var query = new ObtenerClientePorIdQuery()
            {
                Id = key
            };

            var result = await _mediator.Send(query);

            return result.Match(
                        cliente => Ok(_mapper.Map<ClienteDto>(cliente)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Post([FromBody] ClienteDto cliente)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }
            
            var command = new CrearClienteCommand()
            {
                Cliente = _mapper.Map<Cliente>(cliente)
            };

            var result = await _mediator.Send(command);

            return result.Match(
                        result => Created(_mapper.Map<ClienteDto>(result)),
                        errors => Problem(errors));
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<ClienteDto> deltaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }            

            var resultDTO = new ClienteDto();
            deltaDto.Patch(resultDTO);
            var result = _mapper.Map<Cliente>(resultDTO);
            var changeList = deltaDto.GetChangedPropertyNames().ToList();
            var command = new EditarClienteCommand(result, key, changeList);

            var resultActualizacion = await _mediator.Send(command);

            return resultActualizacion.Match(
                updated => Ok(new Tuple<ClienteDto, ClienteDto>(_mapper.Map<ClienteDto>(updated.Item1),
                _mapper.Map<ClienteDto>(updated.Item2))),
                errors => Problem(errors));
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => Error.Validation(description: x.Exception?.Message ?? x.ErrorMessage)).ToList();
                return Problem(errors, false);
            }

            var command = new EliminarClienteCommand()
            {
                IdCliente = key
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => Ok(_mapper.Map<ClienteDto>(deleted)),
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
            IEnumerable<ImportarClienteDto> temp = (IEnumerable<ImportarClienteDto>)param["datos"];
            List<ImportarClienteDto> datos;
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
                Datos = datos.Select(x => new ImportarClienteCommandDto()
                {
                    CodigoCliente = x.CodigoCliente,
                    FechaVencimiento = x.FechaVencimiento,
                    NombreCliente = x.NombreCliente,
                    NumeroIdentificacion = x.NumeroIdentificacion,
                    TipoIdentificacion = x.TipoIdentificacion,
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

            var command = new EliminarClientesCommand()
            {
                IdsClientes = items ?? []
            };

            var result = await _mediator.Send(command);

            return result.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
    }
}
