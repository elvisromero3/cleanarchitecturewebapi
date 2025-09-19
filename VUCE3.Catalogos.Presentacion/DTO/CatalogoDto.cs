using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CatalogoDto
    {
        public int? Id { get; set; }
        public string Alias { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string NombreNormalizado { get; set; } = null!;
        public required bool VisibleEmpresa { get; set; }
        public required bool RequiereFirma { get; set; }
        public string AccionControlador { get; set; } = null!;
        public string NombreIngles { get; set; } = null!;
        public string NombreInglesNormalizado { get; set; } = null!;
        public int? IdFuncionalidad { get; set; }
    }
}
