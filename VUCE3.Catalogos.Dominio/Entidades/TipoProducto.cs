using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class TipoProducto
    {
        public int Id { get; set; }
        public int IdCategoria { get; set; }
        public string Tipo { get; set; } = null!;
        public int IdInstitucion { get; set; }
        public List<CaracteristicaTipoProducto> CaracteristicaTipoProductos { get; set; } = null!;
        public List<ProductoRequisito> ProductoRequisito { get; set; } = null!;

    }
}
