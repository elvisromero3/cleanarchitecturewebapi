using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos
{
    public class ImportarDatosCommand : IRequest<ErrorOr<Created>>
    {
        public int Modo { get; set; }
        public IEnumerable<ImportarCultivoCommandDto> Datos { get; set; } = null!;
    }
}
