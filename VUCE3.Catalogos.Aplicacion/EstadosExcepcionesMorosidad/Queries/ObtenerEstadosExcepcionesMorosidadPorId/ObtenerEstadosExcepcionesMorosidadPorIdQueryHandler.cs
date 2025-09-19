using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidadPorId
{
    public class ObtenerEstadosExcepcionesMorosidadPorIdQueryHandler : IRequestHandler<ObtenerEstadosExcepcionesMorosidadPorIdQuery, ErrorOr<EstadoExcepcionMorosidad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerEstadosExcepcionesMorosidadPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<EstadoExcepcionMorosidad>> Handle(ObtenerEstadosExcepcionesMorosidadPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EstadosExcepcionesMorosidadRepository.ObtenerEstadoExcepcionMorosidadPorId(request.Id);
        }
    }
}
