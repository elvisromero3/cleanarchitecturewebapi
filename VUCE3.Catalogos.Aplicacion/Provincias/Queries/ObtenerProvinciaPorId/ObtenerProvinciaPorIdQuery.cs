using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvinciaPorId
{
    public class ObtenerProvinciaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Provincia>>
    {
        public int IdProvincia { get; set; }
    
    }
}
