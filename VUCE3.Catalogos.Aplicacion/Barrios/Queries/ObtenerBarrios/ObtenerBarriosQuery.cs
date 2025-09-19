using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrios
{
    public class ObtenerBarriosQuery : IRequest<ErrorOr<List<Barrio>>>
    {
    }
}
