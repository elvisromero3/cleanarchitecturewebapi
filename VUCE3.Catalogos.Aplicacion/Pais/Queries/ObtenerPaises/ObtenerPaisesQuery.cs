using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises
{
    public class ObtenerPaisesQuery : IRequest<ErrorOr<List<Dominio.Entidades.Pais>>>
    {
    }
}
