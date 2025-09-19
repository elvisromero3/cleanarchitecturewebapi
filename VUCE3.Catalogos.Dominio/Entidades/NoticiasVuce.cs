using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class NoticiasVuce
    {
        public int Id { get;set; }
        public string Titulo { get; set; } = null!;
        public string Texto { get; set; } = null!;
        public string? Enlace { get; set; }
        public string TituloIngles { get; set; } = null!;
        public string TextoIngles { get;set; } = null!;
    }
}
