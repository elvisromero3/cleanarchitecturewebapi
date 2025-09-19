using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.CrearPaisBloqueComercial
{
    public class CrearPaisBloqueComercialCommand : IRequest<ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        public Dominio.Entidades.PaisBloqueComercial PaisBloqueComercial { get; set; } = null!;
    }
}
