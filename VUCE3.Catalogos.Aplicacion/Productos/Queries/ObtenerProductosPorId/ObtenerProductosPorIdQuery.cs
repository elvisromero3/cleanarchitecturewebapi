using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductosPorId
{
    public class ObtenerProductosPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Productos>>
    {
        public int IdProducto { get; set; }
    }
}
