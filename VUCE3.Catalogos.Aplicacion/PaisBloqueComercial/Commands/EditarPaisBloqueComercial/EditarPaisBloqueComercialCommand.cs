using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial
{
    public class EditarPaisBloqueComercialCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.PaisBloqueComercial, Dominio.Entidades.PaisBloqueComercial>>>
    {
        public Dominio.Entidades.PaisBloqueComercial PaisBloqueComercial { get; set; } = null!;
        public required int IdPaisBloqueComercial { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
