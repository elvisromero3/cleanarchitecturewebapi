using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristicas
{
    public  class EliminarCaracteristicasCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> Ids { get; set; } = null!;
    }
}
