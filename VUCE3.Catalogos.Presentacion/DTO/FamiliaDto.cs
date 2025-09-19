using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class FamiliaDto
    {
        public  int? Id { get; set; }
        
        [JsonRequired]        
        public string Nombre { get; set; } = null!;
    }
}
