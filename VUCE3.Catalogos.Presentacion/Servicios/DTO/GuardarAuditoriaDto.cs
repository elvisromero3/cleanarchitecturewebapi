using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VUCE3.Catalogos.Presentacion.Servicios.DTO
{
    [ExcludeFromCodeCoverage]
    public class GuardarAuditoriaDto
    {
        [Required] public string Pantalla { get; set; } = null!;
        [Required] public string Accion { get; set; } = null!;
        [Required] public int IdUsuario { get; set; }
        [Required] public string RolUsuario { get; set; } = null!;
        public object? DatosAntes { get; set; }
        public object? DatosDespues { get; set; }
        [Required] public int TipoEntidad { get; set; }
        [Required] public int IdEntidad { get; set; }
        public string? IdentificacionEntidad { get; set; }
        public int? IdentificadorRegistroTabla { get; set; }
    }
}
