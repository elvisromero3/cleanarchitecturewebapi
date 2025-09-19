using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EditarAduana
{
    public class EditarAduanaCommand : IRequest<ErrorOr<Tuple<Aduana, Aduana>>>
    {
        public Aduana Aduana { get; set; }
        public int IdAduana { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }

        public EditarAduanaCommand(Aduana Aduana, int idAduana, IEnumerable<string> listaCambio)
        {
            this.Aduana = Aduana;
            this.IdAduana = idAduana;
            this.ListaCambios = listaCambio;
        }
    }
}

