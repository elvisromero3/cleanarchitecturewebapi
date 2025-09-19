using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class ProductoRequisito
    {
        public int Id { get; set; }
        public int IdTipoProducto { get; set; }
        public TipoProducto TipoProducto { get; set; } = null!;
        public int IdRequisito { get; set; }
        public Requisito Requisito { get; set; } = null!;
    }
}
