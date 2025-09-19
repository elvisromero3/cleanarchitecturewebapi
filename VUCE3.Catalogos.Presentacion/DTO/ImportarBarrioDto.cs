using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarBarrioDto
    {
        [JsonRequired] public int Provincia { get; set; }
        [JsonRequired] public int Canton { get; set; }
        [JsonRequired] public int Distrito { get; set; }
        [JsonRequired] public string CodigoBarrio { get; set; } = null!;
        [JsonRequired] public string Barrio { get; set; } = null!;
    }
}
