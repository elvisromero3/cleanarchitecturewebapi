using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Pais.Commands.EditarPais
{
    public class EditarPaisCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Pais, Dominio.Entidades.Pais>>>
    {
        public Dominio.Entidades.Pais Pais { get; set; } = null!;        
        public int IdPais { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }

        public EditarPaisCommand(Dominio.Entidades.Pais Pais, int idPais, IEnumerable<string> listaCambio)
        {
            this.Pais = Pais;
            this.IdPais = idPais;
            this.ListaCambios = listaCambio;
        }
    }
}
