using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloquesComerciales
{
    public class ObtenerBloquesComercialesQueryHandler : IRequestHandler<ObtenerBloquesComercialesQuery, ErrorOr<List<Dominio.Entidades.BloqueComercial>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerBloquesComercialesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.BloqueComercial>>> Handle(ObtenerBloquesComercialesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BloqueComercialRepository.ObtenerBloquesComerciales();
        }
    }
}
