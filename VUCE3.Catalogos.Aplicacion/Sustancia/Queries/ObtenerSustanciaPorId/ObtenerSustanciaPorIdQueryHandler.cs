using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustanciaPorId
{
    public class ObtenerSustanciaPorIdQueryHandler : IRequestHandler<ObtenerSustanciaPorIdQuery, ErrorOr<Dominio.Entidades.Sustancia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerSustanciaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Dominio.Entidades.Sustancia>> Handle(ObtenerSustanciaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SustanciasRepository.ObtenerSustanciaPorId(request.IdSustancia);
        }
    }
}
