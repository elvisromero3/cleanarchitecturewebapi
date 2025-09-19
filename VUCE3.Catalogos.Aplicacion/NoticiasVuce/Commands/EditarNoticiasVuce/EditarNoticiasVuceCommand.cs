using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EditarNoticiasVuce
{
    public class EditarNoticiasVuceCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.NoticiasVuce, Dominio.Entidades.NoticiasVuce>>>
    {
        public Dominio.Entidades.NoticiasVuce NoticiasVuce { get; set; }
        public int IdNoticiasVuce { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }

        public EditarNoticiasVuceCommand(Dominio.Entidades.NoticiasVuce noticiasVuce, int idNoticias, IEnumerable<string> listaCambio)
        {
            this.NoticiasVuce = noticiasVuce;
            this.IdNoticiasVuce = idNoticias;
            this.ListaCambios = listaCambio;
        }
    }
}
