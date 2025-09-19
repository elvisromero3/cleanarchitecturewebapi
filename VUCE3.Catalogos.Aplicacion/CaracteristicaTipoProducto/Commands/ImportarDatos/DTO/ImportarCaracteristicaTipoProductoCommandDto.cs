using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos.DTO
{
    public class ImportarCaracteristicaTipoProductoCommandDto
    {
        public int IdTipoProducto { get; set; }
        public string Caracteristica { get; set; } = null!;
    }
}
