using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class PaisBloqueComercialDto
    {
        public  int? Id { get; set; }
        [JsonRequired] public int IdBloqueComercial { get; set; }
        [JsonRequired] public int IdPais { get; set; }

    }
}
