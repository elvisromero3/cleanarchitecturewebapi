using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CultivoDto
    {
        public int? Id { get; set; }
        [JsonRequired]        
        public string Codigo { get; set; } = null!;        
        public string Nombre { get; set; } = null!;                
        public string NombreCientifico { get; set; } = null!;
        [JsonRequired]        
        public int IdVariedad { get; set; }
    }
}
