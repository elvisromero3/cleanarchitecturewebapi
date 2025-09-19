using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarEmpresaDto
    {
        public string Nombre { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        [JsonRequired] public int IdProfesional { get; set; }

    }
}
