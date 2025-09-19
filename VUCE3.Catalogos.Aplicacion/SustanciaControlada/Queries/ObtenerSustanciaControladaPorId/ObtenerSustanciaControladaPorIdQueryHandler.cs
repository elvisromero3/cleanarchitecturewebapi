using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladaPorId
{
    public class ObtenerSustanciaControladaPorIdQueryHandler : IRequestHandler<ObtenerSustanciaControladaPorIdQuery, ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerSustanciaControladaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> Handle(ObtenerSustanciaControladaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(request.IdSustanciaControlada);
        }
    
    }
}
