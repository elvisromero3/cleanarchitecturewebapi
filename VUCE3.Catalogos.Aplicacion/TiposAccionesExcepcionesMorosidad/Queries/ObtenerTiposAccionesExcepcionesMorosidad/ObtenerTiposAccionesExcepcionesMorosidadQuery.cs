using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidad
{
    public class ObtenerTiposAccionesExcepcionesMorosidadQuery : IRequest<ErrorOr<List<TipoAccionExcepcionMorosidad>>>
    {
    }
}
