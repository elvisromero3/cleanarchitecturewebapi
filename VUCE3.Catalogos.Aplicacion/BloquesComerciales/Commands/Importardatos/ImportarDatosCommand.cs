using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.Importardatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<Dominio.Entidades.BloqueComercial> Datos { get; set; } = null!;
    }
}
