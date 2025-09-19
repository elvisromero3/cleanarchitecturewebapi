using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCanton
{
    public class EliminarCantonCommand : IRequest<ErrorOr<Dominio.Entidades.Canton>>
    {
        public int Id { get; set; }
    }
}
