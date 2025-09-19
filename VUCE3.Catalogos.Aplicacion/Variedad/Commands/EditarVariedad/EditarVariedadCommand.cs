using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.EditarVariedad
{
    public class EditarVariedadCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Variedad, Dominio.Entidades.Variedad>>>
    {
        public EditarVariedadCommand(Dominio.Entidades.Variedad Variedad, int idVariedad, IEnumerable<string> listaCambio)
        {
            this.Variedad = Variedad;
            this.IdVariedad = idVariedad;
            this.ListaCambios = listaCambio;
        }

        public Dominio.Entidades.Variedad Variedad { get; set; }
        public int IdVariedad { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }
    }
}
