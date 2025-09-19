using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionesMorosidad
{
    public class EliminarExcepcionesMorosidadCommand : IRequest<ErrorOr<Deleted>>
    {
        public IEnumerable<int> IdsExcepcionesMorosidad { get; set; } = null!;

    }
}
