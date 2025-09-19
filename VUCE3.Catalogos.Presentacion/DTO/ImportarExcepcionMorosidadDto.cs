using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarExcepcionMorosidadDto
    {
        //Equivalente a tipo trámite
        [JsonRequired] public int Tramite { get; set; }
        
        //Equivalente a subtipo trámite 
        [JsonRequired] public int TipoTramite { get; set; }
        public int? Regimen { get; set; }
        [JsonRequired] public int TipoAccion { get; set; }
        public string Empresa { get; set; } = null!;        
        [JsonRequired] public string FechaInicio { get; set; } = null!;        
        public string? FechaVencimiento { get; set; }
        [JsonRequired] public char TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; } = null!;
        public string Observaciones { get; set; } = null!;        
    }
}
