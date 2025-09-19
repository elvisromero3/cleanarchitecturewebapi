using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Commands.EditarProvincia
{
    public class EditarProvinciaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Provincia, Dominio.Entidades.Provincia>>>
    {
        public Dominio.Entidades.Provincia Provincia { get; set; } = null!;
        public required int IdProvincia { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
