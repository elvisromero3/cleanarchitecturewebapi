using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidadPorId
{
    public class ObtenerTiposAccionesExcepcionesMorosidadPorIdQueryHandler : IRequestHandler<ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery, ErrorOr<TipoAccionExcepcionMorosidad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerTiposAccionesExcepcionesMorosidadPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<TipoAccionExcepcionMorosidad>> Handle(ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TiposAccionesExcepcionesMorosidadRepository.ObtenerTipoAccionExcepcionMorosidadPorId(request.Id);
        }
    }
}
