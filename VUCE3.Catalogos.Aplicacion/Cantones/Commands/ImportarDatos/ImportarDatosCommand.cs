using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarCantonCommandDto> Datos { get; set; } = null!;
    }
}
