namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Cliente
    {
        public int Id { get; set; }
        public string CodigoCliente { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public char IdTipoIdentificacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
