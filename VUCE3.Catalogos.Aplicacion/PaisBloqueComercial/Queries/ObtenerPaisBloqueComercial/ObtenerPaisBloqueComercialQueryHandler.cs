using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial
{
    public class ObtenerPaisBloqueComercialQueryHandler : IRequestHandler<ObtenerPaisBloqueComercialQuery, ErrorOr<List<Dominio.Entidades.PaisBloqueComercial>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerPaisBloqueComercialQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ErrorOr<List<Dominio.Entidades.PaisBloqueComercial>>> Handle(ObtenerPaisBloqueComercialQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial();
        }
    }
}
