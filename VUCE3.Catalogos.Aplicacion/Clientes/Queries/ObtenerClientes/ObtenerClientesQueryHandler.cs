using ErrorOr;
using MediatR;
using System;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientes
{
    public class ObtenerClientesQueryHandler : IRequestHandler<ObtenerClientesQuery, ErrorOr<List<Cliente>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerClientesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Cliente>>> Handle(ObtenerClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _unitOfWork.ClientesRepository.ObtenerClientes();
            return clientes;
        }
    }
}
