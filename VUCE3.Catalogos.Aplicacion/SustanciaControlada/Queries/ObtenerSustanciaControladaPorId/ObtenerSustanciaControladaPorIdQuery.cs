using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladaPorId
{
    public class ObtenerSustanciaControladaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        public int IdSustanciaControlada { get; set; }
    
    }
}
