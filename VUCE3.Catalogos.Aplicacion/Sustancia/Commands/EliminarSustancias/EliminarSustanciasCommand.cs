using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancias
{
    public class EliminarSustanciasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsSustancias { get; set; } = null!;
    }
}
