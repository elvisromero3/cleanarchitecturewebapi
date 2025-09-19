using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EditarTipoProducto
{
    public class EditarTipoProductoCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.TipoProducto, Dominio.Entidades.TipoProducto>>>
    {
        public Dominio.Entidades.TipoProducto TipoProducto { get; set; } = null!;
        public required int IdTipoProducto { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
