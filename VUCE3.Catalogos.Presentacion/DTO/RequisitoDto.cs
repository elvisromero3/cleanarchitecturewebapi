using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class RequisitoDto
    {
        public int? Id { get; set; }
        [JsonRequired] public string Codigo { get; set; } = null!;
        [JsonRequired] public string Descripcion { get; set; } = null!;
        [JsonRequired] public string Version { get; set; } = null!;
        [JsonRequired] public int IdPais { get; set; }
        [JsonRequired] public bool Activo { get; set; }
        [JsonRequired] public int IdInstitucion { get; set; }
        [JsonRequired] public string ImagenRequisito { get; set; } = null!;
        [JsonRequired] public string NombreImagenRequisito { get; set; } = null!;
    }
}
