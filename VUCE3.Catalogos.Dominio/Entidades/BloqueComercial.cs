using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class BloqueComercial
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
