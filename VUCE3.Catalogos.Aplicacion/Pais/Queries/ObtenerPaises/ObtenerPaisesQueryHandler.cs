using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises
{
    public class ObtenerPaisesQueryHandler : IRequestHandler<ObtenerPaisesQuery, ErrorOr<List<Dominio.Entidades.Pais>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerPaisesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Pais>>> Handle(ObtenerPaisesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PaisesRepository.ObtenerPaises();
        }
    }
}
