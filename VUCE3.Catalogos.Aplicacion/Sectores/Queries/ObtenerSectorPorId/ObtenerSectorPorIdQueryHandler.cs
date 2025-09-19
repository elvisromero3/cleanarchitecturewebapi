using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectorPorId
{
    public class ObtenerSectorPorIdQueryHandler : IRequestHandler<ObtenerSectorPorIdQuery, ErrorOr<Sector>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerSectorPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Sector>> Handle(ObtenerSectorPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SectoresRepository.ObtenerSectorPorId(request.IdSector);
        }
    }
}
