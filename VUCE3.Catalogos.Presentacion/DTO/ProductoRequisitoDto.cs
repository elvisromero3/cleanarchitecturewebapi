using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ProductoRequisitoDto
    {
        [JsonRequired]
        public  int Id { get; set; }
        
        [JsonRequired]
        public int IdTipoProducto { get; set; }
        [JsonRequired]
        public int IdRequisito { get; set; }
        public int? IdCategoria { get; set; }
    }
}
