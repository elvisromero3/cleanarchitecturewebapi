
using System.Net.Http.Headers;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class Pais
    {
        public int Id { get; set; }        
        public string Nombre { get; set; } = null!;
        public string CodigoA2 { get; set; } = null!;
        public string CodigoNumerico { get; set; } = null!;
        public string CodigoC3 { get; set; } = null!;

        public List<Requisito> Requisitos { get; set; } = null!;
    }
}
