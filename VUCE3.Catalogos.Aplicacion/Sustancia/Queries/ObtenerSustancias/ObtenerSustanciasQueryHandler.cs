using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustancias
{
    public class ObtenerSustanciasQueryHandler : IRequestHandler<ObtenerSustanciasQuery, ErrorOr<List<Dominio.Entidades.Sustancia>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerSustanciasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<List<Dominio.Entidades.Sustancia>>> Handle(ObtenerSustanciasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SustanciasRepository.ObtenerSustancias();
        }
    }
}
