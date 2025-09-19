using ErrorOr;
using MediatR;


namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<Dominio.Entidades.Establecimiento> Datos { get; set; } = null!;
    }
}
