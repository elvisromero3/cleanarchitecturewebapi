using System.Text.Json.Serialization;
namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class TipoProductoDto
    {
        public int? Id { get; set; }
        [JsonRequired]
        public int IdCategoria { get; set; }
        public string Tipo { get; set; } = null!;
        [JsonRequired]
        public int IdInstitucion { get; set; }
    }
}
