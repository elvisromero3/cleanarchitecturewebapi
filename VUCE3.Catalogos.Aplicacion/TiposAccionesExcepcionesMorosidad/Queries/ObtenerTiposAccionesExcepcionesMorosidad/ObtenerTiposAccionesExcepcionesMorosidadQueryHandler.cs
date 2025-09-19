using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidad
{
    public class ObtenerTiposAccionesExcepcionesMorosidadQueryHandler : IRequestHandler<ObtenerTiposAccionesExcepcionesMorosidadQuery, ErrorOr<List<TipoAccionExcepcionMorosidad>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerTiposAccionesExcepcionesMorosidadQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<TipoAccionExcepcionMorosidad>>> Handle(ObtenerTiposAccionesExcepcionesMorosidadQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TiposAccionesExcepcionesMorosidadRepository.ObtenerTiposAccionesExcepcionesMorosidad();
        }
    }
}
