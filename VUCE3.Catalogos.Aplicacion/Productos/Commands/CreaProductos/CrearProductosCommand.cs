using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.CrearProductos
{
    public class CrearProductosCommand : IRequest<ErrorOr<Dominio.Entidades.Productos>>
    {
        public Dominio.Entidades.Productos Productos { get; set; } = null!;
    }
}
