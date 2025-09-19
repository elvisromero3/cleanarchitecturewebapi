using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidadPorId
{
    public class ObtenerEstadosExcepcionesMorosidadPorIdQuery : IRequest<ErrorOr<EstadoExcepcionMorosidad>>
    {
        public int Id { get; set; }
    }
}
