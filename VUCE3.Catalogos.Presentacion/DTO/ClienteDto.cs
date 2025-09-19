using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ClienteDto
    {
        public int? Id { get; set; }

        [JsonRequired]        

        [Required()]
        public string CodigoCliente { get; set; } = null!;
        
        [JsonRequired]        
        public string NombreCliente { get; set; } = null!;

        [JsonRequired]        
        public string NumeroIdentificacion { get; set; } = null!;

        [JsonRequired]
        public char? IdTipoIdentificacion { get; set; }

        [JsonRequired]
        public DateTime? FechaVencimiento { get; set; }
    }
}
