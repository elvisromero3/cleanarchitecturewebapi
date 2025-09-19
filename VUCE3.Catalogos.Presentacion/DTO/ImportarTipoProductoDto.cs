namespace VUCE3.Catalogos.Presentacion.DTO
{

    public class ImportarTipoProductoDto
    {
        public string Categoria { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string? Institucion { get; set; }
        public int? IdInstitucion { get; set; }
    }
}
