using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int IdInstitucion { get; set; }

    }
}
