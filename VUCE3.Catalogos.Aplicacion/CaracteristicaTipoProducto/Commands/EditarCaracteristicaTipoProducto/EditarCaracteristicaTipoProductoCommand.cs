using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EditarCaracteristicaTipoProducto
{
    public class EditarCaracteristicaTipoProductoCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.CaracteristicaTipoProducto, Dominio.Entidades.CaracteristicaTipoProducto>>>
    {
        public Dominio.Entidades.CaracteristicaTipoProducto CaracteristicaTipoProducto { get; set; } = null!;
        public required int IdCaracteristicaTipoProducto { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
