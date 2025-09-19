using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId
{
    public class ObtenerPaisBloqueComercialPorIdQueryHandler : IRequestHandler<ObtenerPaisBloqueComercialPorIdQuery, ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerPaisBloqueComercialPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> Handle(ObtenerPaisBloqueComercialPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(request.IdPaisBloqueComercial);
        }
    }
}
