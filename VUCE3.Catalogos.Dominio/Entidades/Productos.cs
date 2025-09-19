
namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Productos
    {
        public int Id { get; set; }
        public string Clase { get; set; } = null!;
        public string Presentacion { get; set; } = null!;
        public string NombreComun { get; set; } = null!;
        public string NombreCientifico { get; set; } = null!;
        public bool Tradicional { get; set; }
 
    }
}
