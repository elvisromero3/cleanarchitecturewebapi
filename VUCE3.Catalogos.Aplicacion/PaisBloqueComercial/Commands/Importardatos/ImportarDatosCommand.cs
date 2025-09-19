using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;


namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.Importardatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarPaisBloqueComercialCommandDto> Datos { get; set; } = null!;
    }
}
