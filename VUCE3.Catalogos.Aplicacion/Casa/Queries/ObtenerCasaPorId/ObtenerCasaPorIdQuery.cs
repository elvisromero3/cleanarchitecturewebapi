using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId
{
    public class ObtenerCasaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Casa>>
    {
        public int IdCasa { get; set; }
    
    }
}
