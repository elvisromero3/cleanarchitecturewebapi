using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarImagenes
{
    public class EliminarImagenesCommand : IRequest<ErrorOr<Dominio.Entidades.Imagenes>>
    {
        public int Id { get; set; }
    }
}
