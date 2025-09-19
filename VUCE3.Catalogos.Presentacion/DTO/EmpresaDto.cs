using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class EmpresaDto
    {
        public int? Id { get; set; }

        [JsonRequired]        
        public string Nombre { get; set; } = null!;
        [JsonRequired]        
        public string NumeroIdentificacion { get; set; } = null!;
        [JsonRequired] public char IdTipoIdentificacion { get; set; }
        [JsonRequired] public int IdProfesional { get; set; }
    }
}
