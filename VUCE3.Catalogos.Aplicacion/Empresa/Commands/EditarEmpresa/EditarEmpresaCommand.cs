using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EditarEmpresa
{
    public class EditarEmpresaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Empresa, Dominio.Entidades.Empresa>>>
    {
        public Dominio.Entidades.Empresa Empresa { get; set; } 
        public int IdEmpresa { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }        

        public EditarEmpresaCommand(Dominio.Entidades.Empresa Empresa, int idEmpresa, IEnumerable<string> listaCambio)
        {
            this.Empresa = Empresa;
            this.IdEmpresa = idEmpresa;
            this.ListaCambios = listaCambio;
        }
    }
}
