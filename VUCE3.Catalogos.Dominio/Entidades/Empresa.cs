namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;        
        public char IdTipoIdentificacion { get; set; }
        public int IdProfesional { get; set; }
        public Profesional Profesional { get; set; } = null!;
      
    }
}
