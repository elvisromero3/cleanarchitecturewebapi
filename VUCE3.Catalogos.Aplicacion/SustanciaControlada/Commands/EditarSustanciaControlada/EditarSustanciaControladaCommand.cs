using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EditarSustanciaControlada
{
    public class EditarSustanciaControladaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.SustanciaControlada, Dominio.Entidades.SustanciaControlada>>>
    {
        public Dominio.Entidades.SustanciaControlada SustanciaControlada { get; set; } = null!;
        public required int IdSustanciaControlada { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
