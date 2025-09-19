using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidadPorId
{
    public class ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery : IRequest<ErrorOr<TipoAccionExcepcionMorosidad>>
    {
        public int Id { get; set; }
    }
}
