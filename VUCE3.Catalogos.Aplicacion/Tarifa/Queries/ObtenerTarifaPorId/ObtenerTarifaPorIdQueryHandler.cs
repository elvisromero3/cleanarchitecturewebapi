using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;

namespace VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifaPorId
{
    public class ObtenerTarifaPorIdQueryHandler : IRequestHandler<ObtenerTarifaPorIdQuery, ErrorOr<Dominio.Entidades.Tarifa>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerTarifaPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Tarifa>> Handle(ObtenerTarifaPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TarifasRepository.ObtenerTarifaPorId(request.IdTarifa);
        }
    }
}
