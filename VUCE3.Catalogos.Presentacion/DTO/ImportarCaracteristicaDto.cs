using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarCaracteristicaDto
    {
        public string? Institucion { get; set; } = null!;
        [JsonRequired] public string Nombre { get; set; } = null!;
        public int? IdInstitucion { get; set; }
    }
}
