using MediatR;
using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientos
{
    public class ObtenerEstablecimientosQuery : IRequest<ErrorOr<List<Dominio.Entidades.Establecimiento>>>
    {
    }
}
