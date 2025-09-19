using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Entidades
{
    public class CatalogoInstitucion
    {
        public int Id { get; set; }
        public int CatalogoId { get; set; }
        public int InstitucionId { get; set; }
    }
}
