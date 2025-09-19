using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas
{
    public class ObtenerTarifasQueryHandler : IRequestHandler<ObtenerTarifasQuery, ErrorOr<List<Dominio.Entidades.Tarifa>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ObtenerTarifasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Tarifa>>> Handle(ObtenerTarifasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TarifasRepository.ObtenerTarifas();
        }
    }
}
