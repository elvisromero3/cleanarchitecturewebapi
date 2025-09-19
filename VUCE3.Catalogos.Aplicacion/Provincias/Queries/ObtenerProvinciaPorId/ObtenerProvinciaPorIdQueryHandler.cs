using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvinciaPorId
{
    public class ObtenerProvinciaPorIdQueryHandler : IRequestHandler<ObtenerProvinciaPorIdQuery, ErrorOr<Dominio.Entidades.Provincia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerProvinciaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Provincia>> Handle(ObtenerProvinciaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProvinciaRepository.ObtenerProvinciaPorId(request.IdProvincia);
        }
    
    }
}
