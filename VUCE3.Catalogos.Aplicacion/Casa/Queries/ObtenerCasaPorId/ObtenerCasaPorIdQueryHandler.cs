using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId
{
    public class ObtenerCasaPorIdQueryHandler : IRequestHandler<ObtenerCasaPorIdQuery, ErrorOr<Dominio.Entidades.Casa>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCasaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Casa>> Handle(ObtenerCasaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CasaRepository.ObtenerCasaPorId(request.IdCasa);
        }
    
    }
}
