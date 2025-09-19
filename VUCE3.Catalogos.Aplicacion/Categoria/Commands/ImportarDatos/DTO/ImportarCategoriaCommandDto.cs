namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO
{
    public class ImportarCategoriaCommandDto
    {
        public string Nombre { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public int? IdInstitucion { get; set; }
    }
}
