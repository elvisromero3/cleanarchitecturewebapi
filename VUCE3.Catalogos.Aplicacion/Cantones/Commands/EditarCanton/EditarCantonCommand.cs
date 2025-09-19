using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EditarCanton
{
    public class EditarCantonCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Canton, Dominio.Entidades.Canton>>>
    {
        public Dominio.Entidades.Canton Canton { get; set; } = null!;
        public required int IdCanton { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
