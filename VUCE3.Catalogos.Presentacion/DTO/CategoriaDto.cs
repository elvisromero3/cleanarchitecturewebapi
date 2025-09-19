using System.Text.Json.Serialization;
namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CategoriaDto
    {
        public int? Id { get; set; }
        public string Nombre { get; set; } = null!;
        [JsonRequired] public int IdInstitucion { get; set; }
    }
}
