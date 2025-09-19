using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Tarifa
    {
        public int Id { get; set; }
        public int IdTarifa { get; set; }
        public int IdTipoServicioPlataforma { get; set; }
        public int IdInstitucion { get; set; }
        public string TipoServicioPlataforma { get; set; } = null!;
        public int IdServicioPlataforma { get; set; }
        public string ServicioPlataforma { get; set; } = null!;
        public string CodigoTarifa { get; set; } = null!;
        public string DescripcionTarifa { get; set; } = null!;
        public decimal Importe { get; set; }
        public int IdMoneda { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoTarifa { get; set; } = null!;
        public Moneda Moneda { get; set; } = null!;
    }
}
