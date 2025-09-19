namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos
{
    public class ImportarDatosCaracteristicaDto
    {
        public string Institucion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int? IdInstitucion { get; set; }
    }
}
