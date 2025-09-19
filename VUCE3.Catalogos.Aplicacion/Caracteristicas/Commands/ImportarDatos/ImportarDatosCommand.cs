using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarDatosCaracteristicaDto> Datos { get; set; } = null!;
    }
}
