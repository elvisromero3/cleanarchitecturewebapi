using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Canton
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int IdProvincia { get; set; }
        public Provincia Provincia { get; set; } = null!;
        public List<Distrito> Distritos { get; set; } = null!;
    }
}
