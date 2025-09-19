using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad
{
    public class CrearVariedadCommand : IRequest<ErrorOr<Dominio.Entidades.Variedad>>
    {
        public Dominio.Entidades.Variedad Variedad { get; set; } = null!;
    }
}
