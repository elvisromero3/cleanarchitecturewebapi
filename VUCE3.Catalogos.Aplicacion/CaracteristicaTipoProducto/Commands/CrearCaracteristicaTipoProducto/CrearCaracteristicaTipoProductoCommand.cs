using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.CrearCaracteristicaTipoProducto
{
    public class CrearCaracteristicaTipoProductoCommand : IRequest<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        public Dominio.Entidades.CaracteristicaTipoProducto CaracteristicaTipoProducto { get; set; } = null!;
    }
}
