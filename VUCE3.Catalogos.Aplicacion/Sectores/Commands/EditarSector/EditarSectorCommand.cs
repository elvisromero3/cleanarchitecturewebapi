using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EditarSector
{
    public class EditarSectorCommand : IRequest<ErrorOr<Tuple<Sector, Sector>>>
    {
        public Sector Sector { get; set; } 
        public  int IdSector { get; set; }
        public  List<string> ListaCambios { get; set; }

        public EditarSectorCommand(Sector Sector, int idSector, List<string> listaCambio)
        {
            this.Sector = Sector;
            this.IdSector = idSector;
            this.ListaCambios = listaCambio;
        }
    }
}
