using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class SustanciaControladaDto
    {
        [JsonRequired] public int Id { get; set; }
        public string ClasificacionArancelaria { get; set; } = null!;
        public string ClasificacionAshrae { get; set; } = null!;
        public string PotencialCalentamientoGlobal { get; set; } = null!;
        [JsonRequired] public int IdFamilia { get; set; }
        public string TipoGas {  get; set; } = null!;
    }
}
