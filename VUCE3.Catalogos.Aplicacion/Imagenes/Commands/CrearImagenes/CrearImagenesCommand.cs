using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.CrearImagenes
{
    public class CrearImagenesCommand : IRequest<ErrorOr<Dominio.Entidades.Imagenes>>
    {
        public Dominio.Entidades.Imagenes Imagenes { get; set; } = null!;
    }
}
