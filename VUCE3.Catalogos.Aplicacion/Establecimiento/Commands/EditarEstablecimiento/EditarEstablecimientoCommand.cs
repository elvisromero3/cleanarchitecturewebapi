using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EditarEstablecimiento
{
    public class EditarEstablecimientoCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Establecimiento, Dominio.Entidades.Establecimiento>>>
    {
        public Dominio.Entidades.Establecimiento Establecimiento { get; set; } = default!;
        public int IdEstablecimiento { get; set; }
        public List<string> ListaCambios { get; set; } = default!;
    }
}
