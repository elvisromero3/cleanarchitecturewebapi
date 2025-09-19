using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CaracteristicaTipoProductoDto
    {
        [JsonRequired]
        public  int Id { get; set; }        
        [JsonRequired]
        public int IdTipoProducto { get; set; }
        [JsonRequired]
        public int IdCaracteristica { get; set; }
        public string? NombreCaracteristica { get; set; } 
    }
}
