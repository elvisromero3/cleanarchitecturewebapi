using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidad
{
    public class ObtenerEstadosExcepcionesMorosidadQueryHandler : IRequestHandler<ObtenerEstadosExcepcionesMorosidadQuery, ErrorOr<List<EstadoExcepcionMorosidad>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerEstadosExcepcionesMorosidadQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<EstadoExcepcionMorosidad>>> Handle(ObtenerEstadosExcepcionesMorosidadQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EstadosExcepcionesMorosidadRepository.ObtenerEstadosExcepcionesMorosidad();
        }
    }
}
