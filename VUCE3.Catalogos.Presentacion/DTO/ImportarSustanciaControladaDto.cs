using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarSustanciaControladaDto
    {
        public string ClasificacionArancelaria { get; set; } = null!;
        public string ClasificacionAshrae { get; set; } = null!;
        public string PotencialCalentamientoGlobal { get; set; } = null!;
        public string Familia { get; set; } = null!;
        public string TipoGas { get; set; } = null!;
    }
}
