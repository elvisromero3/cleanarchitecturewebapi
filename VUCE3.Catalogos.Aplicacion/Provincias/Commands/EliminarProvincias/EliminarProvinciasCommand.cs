using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Provincias.Commands.EliminarProvincias
{
    public class EliminarProvinciasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsProvincias { get; set; } = null!;
    }
}
