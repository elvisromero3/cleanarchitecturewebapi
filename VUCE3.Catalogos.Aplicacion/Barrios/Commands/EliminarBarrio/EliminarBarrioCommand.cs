using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrio
{
    public class EliminarBarrioCommand : IRequest<ErrorOr<Barrio>>
    {
        public int Id { get; set; }
    }
}
