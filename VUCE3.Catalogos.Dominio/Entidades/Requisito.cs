namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Requisito
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Version { get; set; } = null!;
        public int IdPais { get; set; }
        public bool Activo { get; set; }
        public int IdInstitucion { get; set; }
        public string ImagenRequisito { get; set; } = null!; // varchar max
        public string NombreImagenRequisito { get; set; } = null!; // varchar 100

        public Pais Pais { get; set; } = null!;
        public List<ProductoRequisito> ProductoRequisito { get; set; } = null!;

    }
}
