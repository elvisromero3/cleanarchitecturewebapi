namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Caracteristica
    {
        public int Id { get; set; }
        public int IdInstitucion { get; set; }
        public string Nombre { get; set; } = null!;
        public List<CaracteristicaTipoProducto> CaracteristicaTipoProductos { get; set; } = null!;
    }
}
