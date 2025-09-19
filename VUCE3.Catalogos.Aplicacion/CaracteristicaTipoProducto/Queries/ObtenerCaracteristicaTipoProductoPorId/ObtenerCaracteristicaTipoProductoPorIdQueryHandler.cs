using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductosPorId
{
    public class ObtenerCaracteristicaTipoProductoPorIdQueryHandler : IRequestHandler<ObtenerCaracteristicaTipoProductoPorIdQuery, ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCaracteristicaTipoProductoPorIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> Handle(ObtenerCaracteristicaTipoProductoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(request.IdCaracteristicaTipoProducto);
        }
    }
}
