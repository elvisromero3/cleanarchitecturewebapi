using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarBarrioCommandDto> Datos { get; set; } = null!;
    }
}
