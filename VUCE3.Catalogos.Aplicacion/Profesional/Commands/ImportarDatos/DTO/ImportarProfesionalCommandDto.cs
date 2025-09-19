using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos.DTO
{
    public class ImportarProfesionalCommandDto
    {
    
        public string Nombre { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; } = null!;
        public string Profesion { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CodigoRegente { get; set; } = null!;
        public int? IdInstitucion { get; set; }
        public string Institucion { get; set; } =null!;
        public bool? Activo { get; set; }
    }
}
