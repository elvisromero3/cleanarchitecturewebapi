using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class SustanciaControlada
    {
        public int Id { get; set; }
        public string ClasificacionArancelaria { get; set; } = null!;
        public string ClasificacionAshrae { get; set; } = null!;
        public string PotencialCalentamientoGlobal { get; set; } = null!;
        public int IdFamilia { get; set; }
        public Familia Familia { get; set; } = null!;
        public string TipoGas { get; set; } = null!;
    

    }
}
