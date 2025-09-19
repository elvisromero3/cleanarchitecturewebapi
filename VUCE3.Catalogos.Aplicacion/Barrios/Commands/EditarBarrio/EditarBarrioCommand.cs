using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EditarBarrio
{
    public class EditarBarrioCommand : IRequest<ErrorOr<Tuple<Barrio, Barrio>>>
    {
        public Barrio Barrio { get; set; } = null!;
        public required int IdBarrio { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
