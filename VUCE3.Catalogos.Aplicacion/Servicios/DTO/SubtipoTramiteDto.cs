using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Servicios.DTO
{
    public class SubtipoTramiteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int IdTipoTramite { get; set; }
    }
}
