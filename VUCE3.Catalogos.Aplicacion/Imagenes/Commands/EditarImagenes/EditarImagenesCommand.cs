using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EditarImagenes
{
    public class EditarImagenesCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Imagenes, Dominio.Entidades.Imagenes>>>
    {
        public Dominio.Entidades.Imagenes Imagenes { get; set; }        
        public int IdImagenes { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }        

        public EditarImagenesCommand(Dominio.Entidades.Imagenes Imagenes, int idImagenes, IEnumerable<string> listaCambio)
        {
            this.Imagenes = Imagenes;
            this.IdImagenes = idImagenes;
            this.ListaCambios = listaCambio;
        }
    }
}
