namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos.DTO
{
    public class ImportarDistritoCommandDto
    {
        public int Provincia { get; set; }
        public int Canton { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
