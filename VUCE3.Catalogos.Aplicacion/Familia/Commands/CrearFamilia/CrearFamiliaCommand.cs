using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.CrearFamilia
{
    public class CrearFamiliaCommand : IRequest<ErrorOr<Dominio.Entidades.Familia>>
    {
        public Dominio.Entidades.Familia Familia { get; set; } = null!;
    }
}
