using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Cultivo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string NombreCientifico { get; set; } = null!;
        public int IdVariedad { get; set; }
        public Variedad Variedad { get; set; } = null!;

    }
}
