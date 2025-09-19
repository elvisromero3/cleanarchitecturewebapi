using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicaPorId
{
    public class ObtenerCaracteristicaPorIdQueryHandler : IRequestHandler<ObtenerCaracteristicaPorIdQuery, ErrorOr<Caracteristica>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCaracteristicaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Caracteristica>> Handle(ObtenerCaracteristicaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicaPorId(request.Id);
        }
    }
}
