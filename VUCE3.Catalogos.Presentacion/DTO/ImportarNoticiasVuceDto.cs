using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarNoticiasVuceDto
    {        
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
