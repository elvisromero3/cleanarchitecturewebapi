namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ProfesionalDto
    {
        public int? Id { get; set; }
        public string Nombre { get; set; } = null!;
        public char? IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; } = null!;
        public string Profesion { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CodigoRegente { get; set; } = null!;
        public int? IdInstitucion { get; set; }
        public bool? Activo { get; set; }

    }
}
