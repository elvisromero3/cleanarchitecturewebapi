using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos.DTO
{
    public class ImportarCultivoCommandDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string NombreCientifico { get; set; } = null!;
        public string CodigoVariedad { get; set; } = null!;
        
    }
}
