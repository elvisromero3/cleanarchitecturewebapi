using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenes
{
    public class ObtenerImagenesQuery : IRequest<ErrorOr<List<Dominio.Entidades.Imagenes>>>
    {
    }
}
