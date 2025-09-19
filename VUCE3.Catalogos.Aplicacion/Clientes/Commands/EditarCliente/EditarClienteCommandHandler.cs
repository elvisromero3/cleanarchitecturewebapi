using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EditarCliente
{
    public class EditarClienteCommandHandler : IRequestHandler<EditarClienteCommand, ErrorOr<Tuple<Cliente, Cliente>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarClienteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Cliente, Cliente>>> Handle(EditarClienteCommand command, CancellationToken cancellationToken)
        {
            var IdTipoIdentificacionString = "IdTipoIdentificacion";
            var NumeroIdentificacionString = "NumeroIdentificacion";
            var CodigoClienteString = "CodigoCliente";
            var NombreClienteString = "NombreCliente";
            var FechaVencimientoString = "FechaVencimiento";

            //Se verifica la existencia de cliente
            var cliente = await _unitOfWork.ClientesRepository.ObtenerClientePorId(command.IdCliente);
            if (cliente.IsError)
            {
                return cliente.Errors;
            }

            var comprobarIdTipoIdentificacion = cliente.Value.IdTipoIdentificacion;
            var comprobarNumeroIdentificacion = cliente.Value.NumeroIdentificacion;
            var comprobarNombreCliente = cliente.Value.NombreCliente;
            var comprobarCodigoCliente = cliente.Value.CodigoCliente;
            var comprobarFechaVencimiento = cliente.Value.FechaVencimiento;
            var comprobarDuplicado = false;

            //se obtiene copia de cliente antes de aplicar cambios
            Cliente clienteAntes = JsonConvert.DeserializeObject<Cliente>(
                JsonConvert.SerializeObject(cliente.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? cliente.Value;

            if (command.ListaCambios.Contains(CodigoClienteString))
            {
                if (Validadores.Codigo35(command.Cliente.CodigoCliente))
                {
                    return ErroresCliente.ClienteTamanoCodigoCliente;
                }
                comprobarCodigoCliente = command.Cliente.CodigoCliente;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains(IdTipoIdentificacionString))
            {
                if (command.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                    command.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                    command.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                    command.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresCliente.TipoIdentificacionNoEncontrado;
                }

                comprobarIdTipoIdentificacion = command.Cliente.IdTipoIdentificacion;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains(NumeroIdentificacionString))
            {
                comprobarNumeroIdentificacion = command.Cliente.NumeroIdentificacion;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains(NumeroIdentificacionString) || command.ListaCambios.Contains(IdTipoIdentificacionString))
            {
                if(Validadores.NumeroIdentificacion(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresCliente.TamanoNroIdentificacion;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresCliente.TipoIdentificacionFisicaComienza0;
                }
            }

            if (command.ListaCambios.Contains(NombreClienteString))
            {
                if (Validadores.Nombre(command.Cliente.NombreCliente))
                {
                    return ErroresCliente.ClienteTamanoNombreCliente;
                }

                comprobarNombreCliente = command.Cliente.NombreCliente;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains(FechaVencimientoString))
            {
                if (command.Cliente.FechaVencimiento.Date <= DateTime.Today)
                {
                    return ErroresCliente.FechaVencimientoMenorHoy;
                }
                comprobarFechaVencimiento = command.Cliente.FechaVencimiento;
                comprobarDuplicado = true;
            }

            //Verifica si existe duplicado
            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.ClientesRepository.ValidarCliente(command.IdCliente, comprobarCodigoCliente, comprobarNombreCliente, comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion, comprobarFechaVencimiento);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCliente.ClienteExiste;
                }
            }

            var result = await _unitOfWork.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return Tuple.Create(clienteAntes, cliente.Value);
        }
    }
}
