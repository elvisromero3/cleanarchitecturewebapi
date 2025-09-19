using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;


namespace VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantonPorId
{
    public class ObtenerCantonPorIdQueryHandler : IRequestHandler<ObtenerCantonPorIdQuery, ErrorOr<Dominio.Entidades.Canton>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCantonPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Canton>> Handle(ObtenerCantonPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CantonesRepository.ObtenerCantonesPorId(request.IdCanton);
        }
    
    }
}
