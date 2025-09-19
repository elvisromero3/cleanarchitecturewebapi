using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionesMorosidad
{
    public class ObtenerExcepcionesMorosidadQueryHandler : IRequestHandler<ObtenerExcepcionesMorosidadQuery, ErrorOr<List<ExcepcionMorosidad>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerExcepcionesMorosidadQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<ExcepcionMorosidad>>> Handle(ObtenerExcepcionesMorosidadQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad();
        }
    }
}
