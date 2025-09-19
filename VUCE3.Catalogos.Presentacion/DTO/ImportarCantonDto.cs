using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarCantonDto
    {
        [JsonRequired] public int Provincia { get; set; }
        [JsonRequired] public string CodigoCanton { get; set; } = null!;
        [JsonRequired] public string Canton { get; set; } = null!;       
    }
}
