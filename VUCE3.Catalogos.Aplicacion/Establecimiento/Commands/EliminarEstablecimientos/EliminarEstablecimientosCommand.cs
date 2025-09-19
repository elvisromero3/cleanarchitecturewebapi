using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimientos
{
    public class EliminarEstablecimientosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsEstablecimientos { get; set; } = null!;
    }
}
