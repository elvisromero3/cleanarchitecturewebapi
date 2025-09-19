using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarBloqueComercialDto
    {
        [JsonRequired] public string Nombre { get; set; } = null!;
    }
}
