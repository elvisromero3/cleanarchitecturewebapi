using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicas
{
    public class ObtenerCaracteristicasQuery : IRequest<ErrorOr<List<Caracteristica>>>
    {
    }
}
