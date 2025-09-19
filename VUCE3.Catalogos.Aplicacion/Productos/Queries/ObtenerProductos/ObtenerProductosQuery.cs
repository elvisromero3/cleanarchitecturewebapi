using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductos
{
    public class ObtenerProductosQuery : IRequest<ErrorOr<List<Dominio.Entidades.Productos>>>
    {
    }
}
