using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Presentacion.DTO
{

    public class ImportarProductoRequisitoDto
    {
        public string Categoria { get; set; } = null!;
        public string TipoProducto { get; set; } = null!;
        public int IdRequisito { get; set; }
    }
}
