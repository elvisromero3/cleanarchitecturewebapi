namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos.DTO
{
    public class ImportarCantonCommandDto
    {
        public int Provincia { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
