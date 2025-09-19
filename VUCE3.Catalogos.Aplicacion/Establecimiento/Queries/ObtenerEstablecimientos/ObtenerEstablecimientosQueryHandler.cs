using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientos
{
    public class ObtenerEstablecimientosQueryHandler : IRequestHandler<ObtenerEstablecimientosQuery, ErrorOr<List<Dominio.Entidades.Establecimiento>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerEstablecimientosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<List<Dominio.Entidades.Establecimiento>>> Handle(ObtenerEstablecimientosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EstablecimientosRepository.ObtenerEstablecimientos();
        }
    }
}
