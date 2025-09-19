using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarClienteDto
    {
        [JsonRequired] public string CodigoCliente { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        [JsonRequired] public DateTime FechaVencimiento { get; set; }
    }
}
