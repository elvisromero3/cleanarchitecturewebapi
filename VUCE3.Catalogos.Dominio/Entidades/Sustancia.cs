using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Sustancia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Cas { get; set; } = null!;
        public string ListaCaq { get; set; } = null!;
    }
}
