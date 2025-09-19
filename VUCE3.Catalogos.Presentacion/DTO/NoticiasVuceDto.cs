using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class NoticiasVuceDto
    {
        public int? Id { get; set; }
        [JsonRequired]        
        public string Titulo { get; set; } = null!;
        [JsonRequired]        
        public string Texto { get; set; } = null!;               
        public string? Enlace { get; set; }
        [JsonRequired]        
        public string TituloIngles { get; set; } = null!;
        [JsonRequired]        
        public string TextoIngles { get; set; } = null!;
    }
}
