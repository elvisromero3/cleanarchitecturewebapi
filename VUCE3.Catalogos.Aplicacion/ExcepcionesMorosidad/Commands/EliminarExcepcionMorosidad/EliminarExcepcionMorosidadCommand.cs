using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionMorosidad
{
    public class EliminarExcepcionMorosidadCommand : IRequest<ErrorOr<ExcepcionMorosidad>>
    {
        public int Id { get; set; }
    }
}
