using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantonPorId
{
    public class ObtenerCantonPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Canton>>
    {
        public int IdCanton { get; set; }
    
    }
}
