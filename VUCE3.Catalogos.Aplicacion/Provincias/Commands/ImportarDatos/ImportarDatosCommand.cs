using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Provincias.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<Dominio.Entidades.Provincia> Datos { get; set; } = null!;
    }
}
