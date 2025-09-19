using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientePorId
{
    public class ObtenerClientePorIdQueryHandler : IRequestHandler<ObtenerClientePorIdQuery, ErrorOr<Cliente>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerClientePorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Cliente>> Handle(ObtenerClientePorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ClientesRepository.ObtenerClientePorId(request.Id);
        }
    }
}
