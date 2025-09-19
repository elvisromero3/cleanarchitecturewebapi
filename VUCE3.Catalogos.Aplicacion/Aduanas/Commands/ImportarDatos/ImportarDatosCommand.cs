using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<Dominio.Entidades.Aduana> Datos { get; set; } = null!;
    }
}
