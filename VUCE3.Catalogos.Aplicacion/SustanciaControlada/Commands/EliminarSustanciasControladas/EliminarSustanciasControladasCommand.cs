using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciasControladas
{
    public class EliminarSustanciasControladasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsSustanciasControladas { get; set; } = null!;
    }
}
