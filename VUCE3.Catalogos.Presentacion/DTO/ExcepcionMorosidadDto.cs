using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ExcepcionMorosidadDto
    {
        public int? Id { get; set; }
        [JsonRequired] public int IdTipoTramite { get; set; }
        [JsonRequired] public int IdSubtipoTramite { get; set; }
        public int? IdRegimen { get; set; }
        [JsonRequired] public int IdTipoAccion { get; set; }
        [JsonRequired] public DateTime FechaInicio { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? IdEstado { get; set; }
        [JsonRequired] public char TipoIdentificacionEmpresa { get; set; } 
        public string NumeroIdentificacionEmpresa { get; set; } = null!;
        public string NombreEmpresa { get; set; } = null!;
        public string Observaciones { get; set; } = null!;
    }
}
