using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarCliente
{
    public class EliminarClienteCommandHandler : IRequestHandler<EliminarClienteCommand, ErrorOr<Cliente>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarClienteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Cliente>> Handle(EliminarClienteCommand command, CancellationToken cancellationToken)
        {
            var cliente = await _unitOfWork.ClientesRepository.ObtenerClientePorId(command.IdCliente);

            if (cliente.IsError)
            {
                return cliente.Errors;
            }
            
            var result = await _unitOfWork.ClientesRepository.EliminarCliente(command.IdCliente);

            if (result.IsError)
            {
                return result.Errors;
            }
                        
            await _unitOfWork.Save();

            return cliente;
        }
    }
}
