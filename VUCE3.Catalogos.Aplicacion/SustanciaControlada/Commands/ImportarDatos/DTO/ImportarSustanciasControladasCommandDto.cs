using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos.DTO
{
    public class ImportarSustanciasControladasCommandDto
    {
        public string ClasificacionArancelaria { get; set; } = null!;
        public string ClasificacionAshrae { get; set; } = null!;
        public string PotencialCalentamientoGlobal { get; set; } = null!;
        public string Familia { get; set; } = null!;
        public string TipoGas { get; set; } = null!;
    }
}
