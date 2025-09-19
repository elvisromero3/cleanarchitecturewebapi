using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCantones
{
    public class EliminarCantonesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsCantones { get; set; } = null!;
    }
}
