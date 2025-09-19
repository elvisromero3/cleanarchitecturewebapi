using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProductos
{
    public class EliminarProductosCommand : IRequest<ErrorOr<Deleted>>
    {
        public required IEnumerable<int> Ids { get; set; }
    }
}
