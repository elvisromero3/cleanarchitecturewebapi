using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EditarBloqueComercial
{
    public class EditarBloqueComercialCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.BloqueComercial, Dominio.Entidades.BloqueComercial>>>
    {
        public Dominio.Entidades.BloqueComercial BloqueComercial { get; set; } = null!;
        public required int IdBloqueComercial { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
