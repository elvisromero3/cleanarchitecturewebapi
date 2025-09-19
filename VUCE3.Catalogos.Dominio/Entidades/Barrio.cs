namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Barrio
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int IdDistrito { get; set; }
        public Distrito Distrito { get; set; } = null!;
    }
}
