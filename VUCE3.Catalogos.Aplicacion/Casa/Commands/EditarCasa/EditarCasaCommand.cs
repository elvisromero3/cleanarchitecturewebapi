using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Casa.Commands.EditarCasa
{
    public class EditarCasaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Casa, Dominio.Entidades.Casa>>>
    {
        public Dominio.Entidades.Casa Casa { get; set; } = null!;
        public required int IdCasa { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
