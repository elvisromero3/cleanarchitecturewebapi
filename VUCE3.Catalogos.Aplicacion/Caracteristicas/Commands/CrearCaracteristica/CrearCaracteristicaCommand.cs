using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.CrearCaracteristica
{
    public class CrearCaracteristicaCommand : IRequest<ErrorOr<Caracteristica>>
    {
        public Caracteristica Caracteristica { get; set; } = null!;
    }
}
