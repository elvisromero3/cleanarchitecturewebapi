using ErrorOr;
using MediatR;


namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.CrearNoticiasVuce
{
    public class CrearNoticiasVuceCommand : IRequest<ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        public Dominio.Entidades.NoticiasVuce NoticiasVuce { get; set; } = null!;
    }
}
