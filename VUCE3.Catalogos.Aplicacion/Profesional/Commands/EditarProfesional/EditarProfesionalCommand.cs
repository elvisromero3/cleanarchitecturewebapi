using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EditarProfesional
{
    public class EditarProfesionalCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Profesional, Dominio.Entidades.Profesional>>>
    {
        public Dominio.Entidades.Profesional Profesional { get; set; }
        public int IdProfesional { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }        

        public EditarProfesionalCommand(Dominio.Entidades.Profesional Profesional, int idProfesional, IEnumerable<string> listaCambio)
        {
            this.Profesional = Profesional;
            this.IdProfesional = idProfesional;
            this.ListaCambios = listaCambio;
        }
    }
}
