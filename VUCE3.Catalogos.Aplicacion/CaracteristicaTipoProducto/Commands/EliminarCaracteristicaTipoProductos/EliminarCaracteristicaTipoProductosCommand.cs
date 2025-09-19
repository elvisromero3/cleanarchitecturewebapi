using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProductos
{
    public class EliminarCaracteristicaTipoProductosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsCaracteristicaTipoProductos { get; set; } = null!;
    }
}
