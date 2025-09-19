using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientoPorId
{
    public class ObtenerEstablecimientosPorIdQueryHandler : IRequestHandler<ObtenerEstablecimientosPorIdQuery, ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerEstablecimientosPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Dominio.Entidades.Establecimiento>> Handle(ObtenerEstablecimientosPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EstablecimientosRepository.ObtenerEstablecimientoPorId(request.IdEstablecimiento);
        }
    }
}
