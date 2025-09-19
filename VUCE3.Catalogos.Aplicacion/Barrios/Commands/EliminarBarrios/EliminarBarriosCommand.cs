using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrios
{
    public class EliminarBarriosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsBarrios { get; set; } = null!;
    }
}
