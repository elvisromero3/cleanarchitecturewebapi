using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarProvinciaDto
    {
        [JsonRequired] public string CodigoProvincia { get; set; } = null!;
        [JsonRequired] public string Provincia { get; set; } = null!;
    }
}
