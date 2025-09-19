using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladas
{
    public class ObtenerSustanciaControladasQueryHandler : IRequestHandler<ObtenerSustanciaControladasQuery, ErrorOr<List<Dominio.Entidades.SustanciaControlada>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerSustanciaControladasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.SustanciaControlada>>> Handle(ObtenerSustanciaControladasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladas();
        }
    }
}
