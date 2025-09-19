using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.EliminarBloquesComerciales
{
    public class EliminarBloquesComercialesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsBloquesComerciales { get; set; } = null!;
    }
}
