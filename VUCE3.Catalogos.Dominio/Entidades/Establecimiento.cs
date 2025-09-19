using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Establecimiento
    {
        public int Id { get; set; }
        public string NumeroCvo { get; set; } = null!;
        public string NombreEstablecimiento { get; set; } = null!;
        public string ActividadPrimaria { get; set; } = null!;
        public string? ActividadSecundaria { get; set; }
        public string Provincia { get; set; } = null!;
        public string Canton { get; set; } = null!;
        public string Distrito { get; set; } = null!;
        public string DireccionExacta { get; set; } = null!;
        public DateTime FechaVencimiento { get; set; }
    
        public string EstadoEstablecimiento { get; set; } = null!;
    }
}
