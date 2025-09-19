using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionesMorosidad
{
    public class ObtenerExcepcionesMorosidadQuery : IRequest<ErrorOr<List<ExcepcionMorosidad>>>
    {
    }
}
