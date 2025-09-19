using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Servicios.DTO
{

    public class AduanaInstitucionUsuarioDto
    {
        public int? Id { get; set; }
        public int IdInstitucionAutorizada { get; set; }
        public int IdAduana { get; set; }

    }
}
