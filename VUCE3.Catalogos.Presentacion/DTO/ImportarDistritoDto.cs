using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarDistritoDto
    {
        [JsonRequired] public int Provincia { get; set; }
        [JsonRequired] public int Canton { get; set; }
        [JsonRequired] public string CodigoDistrito { get; set; } = null!;
        [JsonRequired] public string Distrito { get; set; } = null!;
    }
}
