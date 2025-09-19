namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class CantonDto
    {
        public int? Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int? IdProvincia { get; set; }
    }
}
