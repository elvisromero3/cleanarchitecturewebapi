using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ProductosDto
    {
        public  int? Id { get; set; }
        [JsonRequired]
        public string Clase { get; set; } = null!;
        [JsonRequired]
        public string Presentacion { get; set; } = null!;
        [JsonRequired]
        public string NombreComun { get; set; } = null!;
        [JsonRequired]
        public string NombreCientifico { get; set; } = null!;
        public bool? Tradicional { get; set; }
    }
}
