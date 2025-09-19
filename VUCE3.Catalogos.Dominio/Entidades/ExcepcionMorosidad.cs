namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class ExcepcionMorosidad
    {
        public int Id { get; set; }
        public int IdTipoTramite { get; set; }
        public int IdSubtipoTramite { get; set; }
        public int? IdRegimen { get; set; }
        public int IdTipoAccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int IdEstado { get; set; }
        public char TipoIdentificacionEmpresa { get; set; }
        public string NumeroIdentificacionEmpresa { get; set; } = null!;
        public string NombreEmpresa { get; set; } = null!;
        public string Observaciones { get; set; } = null!;

        public TipoAccionExcepcionMorosidad TipoAccionExcepcionMorosidad { get; set; } = null!;
        public EstadoExcepcionMorosidad EstadoExcepcionMorosidad { get; set; } = null!;
    }
}
