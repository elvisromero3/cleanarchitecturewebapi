using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.CrearCaracteristicaTipoProducto
{
    public class CrearCaracteristicaTipoProductoCommandHandler : IRequestHandler<CrearCaracteristicaTipoProductoCommand, ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearCaracteristicaTipoProductoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>> Handle(CrearCaracteristicaTipoProductoCommand request, CancellationToken cancellationToken)
        {
            var existe = await _unitOfWork.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(request.CaracteristicaTipoProducto.IdTipoProducto,request.CaracteristicaTipoProducto.IdCaracteristica);
            if (existe.Value)
            {
                return ErroresCaracteristicaTipoProducto.DatosDuplicados;
            }

            var result = await _unitOfWork.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(request.CaracteristicaTipoProducto);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }

    }
}
