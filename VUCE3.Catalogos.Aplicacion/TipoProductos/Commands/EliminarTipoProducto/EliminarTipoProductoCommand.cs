using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EliminarTipoProducto
{
    public class EliminarTipoProductoCommand : IRequest<ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        public int Id { get; set; }
    }
}
