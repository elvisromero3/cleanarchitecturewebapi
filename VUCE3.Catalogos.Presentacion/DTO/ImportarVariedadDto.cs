using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarVariedadDto
    {
        
        [JsonRequired]        
        public string Codigo { get; set; } = null!;

        [JsonRequired]        
        public string Nombre { get; set; } = null!;
        
    }
}
