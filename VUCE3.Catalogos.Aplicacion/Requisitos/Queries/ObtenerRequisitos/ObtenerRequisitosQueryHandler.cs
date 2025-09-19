using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitos
{
    public class ObtenerRequisitosQueryHandler : IRequestHandler<ObtenerRequisitosQuery, ErrorOr<List<Requisito>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerRequisitosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Requisito>>> Handle(ObtenerRequisitosQuery request, CancellationToken cancellationToken)
        {
            var requisitos = await _unitOfWork.RequisitosRepository.ObtenerRequisitos();
            return requisitos;
        }
    }
}
