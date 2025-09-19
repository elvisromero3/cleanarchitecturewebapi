using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos.DTO
{
    public class ImportarClienteCommandDto
    {
        public string CodigoCliente { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
