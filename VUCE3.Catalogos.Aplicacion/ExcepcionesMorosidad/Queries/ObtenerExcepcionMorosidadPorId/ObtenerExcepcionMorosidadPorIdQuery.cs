using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionMorosidadPorId
{
    public class ObtenerExcepcionMorosidadPorIdQuery : IRequest<ErrorOr<ExcepcionMorosidad>>
    {
        public int IdExcepcionMorosidad { get; set; }
    }
}
