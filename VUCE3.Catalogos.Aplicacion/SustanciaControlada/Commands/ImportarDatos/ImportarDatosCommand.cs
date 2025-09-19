using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarSustanciasControladasCommandDto> Datos { get; set; } = null!;
    }
}
