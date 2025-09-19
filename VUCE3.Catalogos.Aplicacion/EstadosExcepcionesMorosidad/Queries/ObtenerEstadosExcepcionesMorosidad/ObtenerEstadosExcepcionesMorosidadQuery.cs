using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidad
{
    public class ObtenerEstadosExcepcionesMorosidadQuery : IRequest<ErrorOr<List<EstadoExcepcionMorosidad>>>
    {
    }
}
