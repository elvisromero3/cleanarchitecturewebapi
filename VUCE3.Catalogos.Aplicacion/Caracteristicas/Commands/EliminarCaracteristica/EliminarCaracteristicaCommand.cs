using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristica
{
    public class EliminarCaracteristicaCommand : IRequest<ErrorOr<Caracteristica>>
    {
        public int Id { get; set; }
    }
}
