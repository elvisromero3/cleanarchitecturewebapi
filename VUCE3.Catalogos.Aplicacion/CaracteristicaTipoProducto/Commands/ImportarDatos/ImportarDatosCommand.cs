using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarCaracteristicaTipoProductoCommandDto> Datos { get; set; } = null!;
    }
}
