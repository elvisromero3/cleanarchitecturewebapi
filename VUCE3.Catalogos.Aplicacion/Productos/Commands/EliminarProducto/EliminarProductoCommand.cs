using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProducto
{
    public class EliminarProductoCommand : IRequest<ErrorOr<Dominio.Entidades.Productos>>
    {
        public int Id { get; set; }
    }
}
