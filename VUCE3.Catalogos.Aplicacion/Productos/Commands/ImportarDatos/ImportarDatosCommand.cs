using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Productos.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<Dominio.Entidades.Productos> Datos { get; set; } = null!;
    }
}
