using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;

namespace VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedas
{
    public class ObtenerMonedasQueryHandler : IRequestHandler<ObtenerMonedasQuery, ErrorOr<List<Dominio.Entidades.Moneda>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerMonedasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Moneda>>> Handle(ObtenerMonedasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.MonedasRepository.ObtenerMonedas();
        }
    }
}
