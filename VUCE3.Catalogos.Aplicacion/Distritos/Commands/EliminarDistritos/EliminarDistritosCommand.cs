using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos
{
    public class EliminarDistritosCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsDistritos { get; set; } = null!;
    }
}
