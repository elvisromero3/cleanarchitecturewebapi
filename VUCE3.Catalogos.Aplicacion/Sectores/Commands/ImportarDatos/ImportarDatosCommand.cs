using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarSectorCommandDto> Datos { get; set; } = null!;
    }
}
