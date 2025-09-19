using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EditarProductos
{
    public class EditarProductosCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Productos, Dominio.Entidades.Productos>>>
    {
        public Dominio.Entidades.Productos Productos { get; set; } = null!;
        public required int IdProducto { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
