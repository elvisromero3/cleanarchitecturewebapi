using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId
{
    public class ObtenerPaisPorIdQueryHandler : IRequestHandler<ObtenerPaisPorIdQuery, ErrorOr<Dominio.Entidades.Pais>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerPaisPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Pais>> Handle(ObtenerPaisPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PaisesRepository.ObtenerPaisPorId(request.Id);
        }
    }
}
