using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloquesComerciales
{
    public class EliminarPaisBloquesComercialesCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsBloquesComerciales { get; set; } = null!;
    }
}
