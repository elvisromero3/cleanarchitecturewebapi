using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class BarrioDto
    {
        public int? Id { get; set; }
        [JsonRequired] public string Codigo { get; set; } = null!;
        [JsonRequired] public string Nombre { get; set; } = null!;
        [JsonRequired] public int IdDistrito { get; set; }
        [JsonRequired] public int IdCanton { get; set; }
        [JsonRequired] public int IdProvincia { get; set; }        
    }
}
