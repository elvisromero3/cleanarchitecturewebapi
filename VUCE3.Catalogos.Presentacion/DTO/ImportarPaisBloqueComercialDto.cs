using System.Text.Json.Serialization;

namespace VUCE3.Catalogos.Presentacion.DTO
{
    public class ImportarPaisBloqueComercialDto
    {
        
        public string BloqueComercial { get; set; } = null!;
        public string Pais { get; set; } = null!;

    }
}
