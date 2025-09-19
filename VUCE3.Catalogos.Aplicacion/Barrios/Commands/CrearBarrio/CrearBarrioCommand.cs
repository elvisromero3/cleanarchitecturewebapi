using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.CrearBarrio
{
    public class CrearBarrioCommand : IRequest<ErrorOr<Barrio>>
    {
        public Barrio Barrio { get; set; } = null!;
    }
}
