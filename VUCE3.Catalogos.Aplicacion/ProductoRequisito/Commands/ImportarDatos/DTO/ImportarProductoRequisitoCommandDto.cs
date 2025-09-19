using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO
{
    public class ImportarProductoRequisitoCommandDto
    {
        public string Categoria { get; set; } = null!;
        public string TipoProducto { get; set; } = null!;
        public int IdRequisito { get; set; }
    }
}
