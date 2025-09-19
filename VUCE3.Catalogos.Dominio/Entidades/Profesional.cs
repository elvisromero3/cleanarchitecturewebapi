namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Profesional
    {        
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public char IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; } = null!;
        public string Profesion { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CodigoRegente { get; set; } = null!;
        public bool Activo { get; set; }
        public int IdInstitucion { get; set; }
        public List<Empresa> Empresas { get; set; } = null!;
    }
}
