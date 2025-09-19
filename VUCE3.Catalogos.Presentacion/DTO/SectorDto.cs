using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class SectorDto
    {
        public int? Id { get; set; }
        [JsonRequired] public string Nombre { get; set; } = null!;
        [JsonRequired] public string Codigo { get; set; } = null!;  
    }
}
