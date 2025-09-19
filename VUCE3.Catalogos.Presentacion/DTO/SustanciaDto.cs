using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class SustanciaDto
    {
        [JsonRequired] public int? Id { get; set; }
        [JsonRequired] public string Nombre { get; set; } = null!;
        [JsonRequired] public string Cas { get; set; } = null!;
        [JsonRequired] public string ListaCaq { get; set; } = null!;
    }
}
