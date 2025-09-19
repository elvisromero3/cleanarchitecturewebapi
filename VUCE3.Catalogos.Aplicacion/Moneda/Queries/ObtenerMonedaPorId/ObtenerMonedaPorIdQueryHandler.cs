using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;

namespace VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedaPorId
{
    public class ObtenerMonedaPorIdQueryHandler : IRequestHandler<ObtenerMonedaPorIdQuery, ErrorOr<Dominio.Entidades.Moneda>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerMonedaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Moneda>> Handle(ObtenerMonedaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.MonedasRepository.ObtenerMonedaPorId(request.IdMoneda);
        }
    }
}
