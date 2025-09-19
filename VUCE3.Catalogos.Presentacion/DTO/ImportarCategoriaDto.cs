namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarCategoriaDto
    {
        public string Nombre { get; set; } = null!;
        public int? IdInstitucion { get; set; }
        public string? Institucion { get; set; }
    }
}
