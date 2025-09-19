namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Catalogo
    {
        public int Id { get; set; }
        public string Alias { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string NombreNormalizado { get; set; } = null!;
        public bool VisibleEmpresa { get; set; }
        public bool RequiereFirma { get; set; }
        public string AccionControlador { get; set; } = null!;
        public string NombreIngles { get; set; } = null!;
        public string NombreInglesNormalizado { get; set; } = null!;
        public int IdFuncionalidad { get; set; }
        public ICollection<CatalogoInstitucion> CatalogoInstituciones { get; set; } = new List<CatalogoInstitucion>();
    }
}
