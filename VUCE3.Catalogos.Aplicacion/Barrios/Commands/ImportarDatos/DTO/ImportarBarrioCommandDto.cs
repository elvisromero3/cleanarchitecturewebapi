namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos.DTO
{
    public class ImportarBarrioCommandDto
    {
        public int Provincia { get; set; }
        public int Canton { get; set; }
        public int Distrito { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
