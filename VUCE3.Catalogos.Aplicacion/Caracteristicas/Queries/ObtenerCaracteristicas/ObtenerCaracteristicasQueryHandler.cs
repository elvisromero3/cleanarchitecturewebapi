using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicas
{
    public class ObtenerCaracteristicasQueryHandler : IRequestHandler<ObtenerCaracteristicasQuery, ErrorOr<List<Caracteristica>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerCaracteristicasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Caracteristica>>> Handle(ObtenerCaracteristicasQuery request, CancellationToken cancellationToken)
        {
            var caracteristicas = await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicas();
            return caracteristicas;
        }
    }
}
