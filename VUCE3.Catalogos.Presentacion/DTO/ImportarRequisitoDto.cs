using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Presentacion.DTO
{

    public class ImportarRequisitoDto
    {
        public string Codigo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string Pais { get; set; } = null!;
        public int IdInstitucion { get; set; }
    }
}
