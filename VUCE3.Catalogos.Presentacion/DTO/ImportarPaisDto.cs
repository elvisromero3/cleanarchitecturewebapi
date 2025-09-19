using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarPaisDto
    {
        
        [JsonRequired]        
        public string Nombre { get; set; } = null!;
        
        [JsonRequired]        
        public string CodigoA2 { get; set; } = null!;
        
        [JsonRequired]        
        public string CodigoNumerico { get; set; } = null!;
        
        [JsonRequired]        
        public string CodigoC3 { get; set; } = null!;
    }
}
