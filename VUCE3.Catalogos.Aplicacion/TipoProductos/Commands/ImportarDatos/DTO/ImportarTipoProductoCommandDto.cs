namespace VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO
{
    public class ImportarTipoProductoCommandDto
    {
        public string Categoria { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public int? IdInstitucion { get; set; }
    }
}
