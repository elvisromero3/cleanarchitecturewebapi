using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductos
{
    public class ObtenerCaracteristicaTipoProductosQueryHandler : IRequestHandler<ObtenerCaracteristicaTipoProductoQuery, ErrorOr<List<Dominio.Entidades.CaracteristicaTipoProducto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerCaracteristicaTipoProductosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ErrorOr<List<Dominio.Entidades.CaracteristicaTipoProducto>>> Handle(ObtenerCaracteristicaTipoProductoQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos();
        }
    }
}
