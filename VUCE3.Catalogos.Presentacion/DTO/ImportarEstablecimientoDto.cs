using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarEstablecimientoDto
    {
        [JsonRequired] public string NumeroCvo { get; set; } = null!;
        [JsonRequired] public string NombreEstablecimiento { get; set; } = null!;
        [JsonRequired] public string ActividadPrimaria { get; set; } = null!;
        public string? ActividadSecundaria { get; set; }
        [JsonRequired] public string Provincia { get; set; } = null!;
        [JsonRequired] public string Canton { get; set; } = null!;
        [JsonRequired] public string Distrito { get; set; } = null!;
        [JsonRequired] public string DireccionExacta { get; set; } = null!;
        [JsonRequired] public string FechaVencimiento { get; set; } = null!;
        [JsonRequired] public string EstadoEstablecimiento { get; set; } = null!;
    }
}
