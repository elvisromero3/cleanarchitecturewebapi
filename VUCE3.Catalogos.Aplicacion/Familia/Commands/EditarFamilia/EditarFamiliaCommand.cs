using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EditarFamilia
{
    public class EditarFamiliaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Familia, Dominio.Entidades.Familia>>>
    {
        public Dominio.Entidades.Familia Familia { get; set; } = null!;
        public required int IdFamilia { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
