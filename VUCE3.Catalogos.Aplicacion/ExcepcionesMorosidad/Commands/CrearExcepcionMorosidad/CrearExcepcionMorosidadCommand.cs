using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.CrearExcepcionMorosidad
{
    public class CrearExcepcionMorosidadCommand : IRequest<ErrorOr<ExcepcionMorosidad>>
    {
        public ExcepcionMorosidad ExcepcionMorosidad { get; set; } = null!;
    }
}
