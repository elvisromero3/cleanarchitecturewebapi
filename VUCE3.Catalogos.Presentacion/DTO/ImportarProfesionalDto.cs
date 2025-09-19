namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarProfesionalDto
    {
    
        public string Nombre { get; set; } = null!;
        public char TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; } = null!;
        public string Profesion { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CodigoRegente { get; set; } = null!;
        public bool Activo { get; set; }
        public int? IdInstitucion { get; set; }
        public string? Institucion { get; set; } = null!;

    }
}
