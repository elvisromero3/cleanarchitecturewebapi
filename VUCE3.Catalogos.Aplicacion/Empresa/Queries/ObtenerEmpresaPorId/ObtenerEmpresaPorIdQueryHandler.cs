using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresaPorId
{
    public class ObtenerEmpresaPorIdQueryHandler : IRequestHandler<ObtenerEmpresaPorIdQuery, ErrorOr<Dominio.Entidades.Empresa>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerEmpresaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Empresa>> Handle(ObtenerEmpresaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EmpresasRepository.ObtenerEmpresaPorId(request.Id);
        }
    }
}
