using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitoPorId
{
    public class ObtenerRequisitoPorIdQueryhandler : IRequestHandler<ObtenerRequisitoPorIdQuery, ErrorOr<Requisito>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerRequisitoPorIdQueryhandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Requisito>> Handle(ObtenerRequisitoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.RequisitosRepository.ObtenerRequisitoPorId(request.Id);
        }
    }
}
