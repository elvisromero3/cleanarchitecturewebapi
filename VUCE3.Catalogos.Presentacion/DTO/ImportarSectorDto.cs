using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarSectorDto
    {
        [JsonRequired] public string Nombre { get; set; } = null!;
        [JsonRequired] public string Codigo { get; set; } = null!;
    }
}
