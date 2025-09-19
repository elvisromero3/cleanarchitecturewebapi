using System.Text.Json.Serialization;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class EstablecimientoDto
    {
        public int? Id { get; set; }
        [JsonRequired] public string NumeroCvo { get; set; } = null!;
        [JsonRequired] public string NombreEstablecimiento { get; set; } = null!;
        [JsonRequired] public string ActividadPrimaria { get; set; } = null!;
        [JsonRequired] public string? ActividadSecundaria { get; set; }
        [JsonRequired] public string Provincia { get; set; } = null!;
        [JsonRequired] public string Canton { get; set; } = null!;
        [JsonRequired] public string Distrito { get; set; } = null!;
        [JsonRequired] public string DireccionExacta { get; set; } = null!;
        [JsonRequired] public DateTime FechaVencimiento { get; set; }
        [JsonRequired] public string EstadoEstablecimiento { get; set; } = null!;
    }
}
