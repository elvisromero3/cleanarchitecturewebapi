using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarDistritoCommandDto> Datos { get; set; } = null!;
    }
}
