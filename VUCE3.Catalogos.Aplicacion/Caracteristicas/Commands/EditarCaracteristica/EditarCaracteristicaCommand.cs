using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EditarCaracteristica
{
    public class EditarCaracteristicaCommand : IRequest<ErrorOr<Tuple<Caracteristica, Caracteristica>>>
    {
        public Caracteristica Caracteristica { get; set; }
        public int IdCaracteristica { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }

        public EditarCaracteristicaCommand(Caracteristica Caracteristica, int idCaracteristica, IEnumerable<string> listaCambio)
        {
            this.Caracteristica = Caracteristica;
            this.IdCaracteristica = idCaracteristica;
            this.ListaCambios = listaCambio;
        }
    }
}
