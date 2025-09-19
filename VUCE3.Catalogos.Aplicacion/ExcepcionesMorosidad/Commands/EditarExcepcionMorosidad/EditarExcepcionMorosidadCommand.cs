using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EditarExcepcionMorosidad
{
    public class EditarExcepcionMorosidadCommand : IRequest<ErrorOr<Tuple<ExcepcionMorosidad, ExcepcionMorosidad>>>
    {
        public ExcepcionMorosidad ExcepcionMorosidad { get; set; } = null!;
        public required int IdExcepcionMorosidad { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
