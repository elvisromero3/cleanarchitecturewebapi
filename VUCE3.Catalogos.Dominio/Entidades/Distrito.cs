namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Distrito
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int IdCanton { get; set; }
        public Canton Canton { get; set; } = null!;
        public List<Barrio> Barrios { get; set; } = null!;
    }
}
