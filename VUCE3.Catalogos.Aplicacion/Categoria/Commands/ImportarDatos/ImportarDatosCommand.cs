using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarCategoriaCommandDto> Datos { get; set; } = null!;
    }
}
