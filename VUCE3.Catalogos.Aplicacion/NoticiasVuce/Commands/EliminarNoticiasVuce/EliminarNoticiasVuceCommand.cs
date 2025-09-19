using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarNoticiasVuce
{
    public class EliminarNoticiasVuceCommand : IRequest<ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        public int Id { get; set; }
    }
}
