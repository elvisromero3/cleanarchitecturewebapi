using System.Text.Json.Serialization;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class DistritoDto
    {
        public int? Id { get; set; }
        [JsonRequired] public string Codigo { get; set; } = null!;
        [JsonRequired] public string Nombre { get; set; } = null!;
        [JsonRequired] public int IdCanton { get; set; }
        [JsonRequired] public int IdProvincia { get; set; }
    }
}
