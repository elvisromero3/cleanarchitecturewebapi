using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CaracteristicaDto
    {
        public int? Id { get; set; }
        [JsonRequired] public int? IdInstitucion { get; set; }
        [JsonRequired] public string Nombre { get; set; } = null!;
    }
}
