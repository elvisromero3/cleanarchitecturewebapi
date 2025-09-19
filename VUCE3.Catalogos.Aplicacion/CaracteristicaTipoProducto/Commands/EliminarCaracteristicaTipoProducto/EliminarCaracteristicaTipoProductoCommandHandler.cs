using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto
{
    public class EliminarCaracteristicaTipoProductoCommandHandler : IRequestHandler<EliminarCaracteristicaTipoProductoCommand, ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCaracteristicaTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> Handle(EliminarCaracteristicaTipoProductoCommand command, CancellationToken cancellationToken)
        {
            var familia = await _unitOfWork.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(command.Id);

            if (familia.IsError)
            {
                return familia.Errors;
            }

            var result = await _unitOfWork.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return familia;
        }
    }
}
