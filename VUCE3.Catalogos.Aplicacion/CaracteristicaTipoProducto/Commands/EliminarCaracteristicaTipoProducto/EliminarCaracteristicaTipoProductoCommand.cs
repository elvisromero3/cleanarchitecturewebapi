using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto
{
    public class EliminarCaracteristicaTipoProductoCommand : IRequest<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        public int Id { get; set; }
    }
}
