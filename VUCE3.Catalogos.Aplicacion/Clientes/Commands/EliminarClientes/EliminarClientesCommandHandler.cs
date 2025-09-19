using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarClientes
{
    public class EliminarClientesCommandHandler : IRequestHandler<EliminarClientesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarClientesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarClientesCommand request, CancellationToken cancellationToken)
        {
            foreach(var idCliente in request.IdsClientes)
            {
                var resultDelete = await _unitOfWork.ClientesRepository.EliminarCliente(idCliente);

                if (resultDelete.IsError)
                {
                    return resultDelete.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
