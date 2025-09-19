using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias
{
    public class EliminarFamiliasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsFamilias { get; set; } = null!;
    }
}
