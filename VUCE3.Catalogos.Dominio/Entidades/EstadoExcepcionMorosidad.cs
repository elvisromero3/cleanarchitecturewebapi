namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class EstadoExcepcionMorosidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public List<ExcepcionMorosidad> ExcepcionesMorosidad { get; set; } = null!;
    }
}
