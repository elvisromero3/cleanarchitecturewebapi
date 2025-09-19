using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenesPorId
{
    public class ObtenerImagenesPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Imagenes>>
    {
        public int Id { get; set; }
    }
}
