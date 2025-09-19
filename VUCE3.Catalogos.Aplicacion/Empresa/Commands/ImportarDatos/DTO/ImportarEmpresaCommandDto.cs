using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos.DTO
{
    public class ImportarEmpresaCommandDto
    {
        public string Nombre { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        public int IdProfesional { get; set; }

    }
}
