using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionMorosidadPorId
{
    public class ObtenerExcepcionMorosidadPorIdQueryHandler : IRequestHandler<ObtenerExcepcionMorosidadPorIdQuery, ErrorOr<ExcepcionMorosidad>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerExcepcionMorosidadPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> Handle(ObtenerExcepcionMorosidadPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(request.IdExcepcionMorosidad);
        }
    }
}
