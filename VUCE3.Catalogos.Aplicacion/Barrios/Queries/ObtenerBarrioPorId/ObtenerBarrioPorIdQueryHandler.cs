using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrioPorId
{
    public class ObtenerBarrioPorIdQueryHandler : IRequestHandler<ObtenerBarrioPorIdQuery, ErrorOr<Barrio>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerBarrioPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Barrio>> Handle(ObtenerBarrioPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BarriosRepository.ObtenerBarrioPorId(request.Id);
        }
    }
}
