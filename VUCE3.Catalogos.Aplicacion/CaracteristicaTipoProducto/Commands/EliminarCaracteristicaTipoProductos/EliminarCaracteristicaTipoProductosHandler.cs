using ErrorOr;
using MediatR;

using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProductos
{
    public class EliminarCaracteristicaTipoProductosCommandHandler :IRequestHandler<EliminarCaracteristicaTipoProductosCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarCaracteristicaTipoProductosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarCaracteristicaTipoProductosCommand request, CancellationToken cancellationToken)
        {
            foreach (var idCaracteristicaTipoProducto in request.IdsCaracteristicaTipoProductos)
            {
                var result = await _unitOfWork.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(idCaracteristicaTipoProducto);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
