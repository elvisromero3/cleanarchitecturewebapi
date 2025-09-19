using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarProductoDto
    {
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
