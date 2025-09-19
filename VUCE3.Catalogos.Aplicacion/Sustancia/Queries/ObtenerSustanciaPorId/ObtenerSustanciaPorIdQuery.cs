using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustanciaPorId
{
    public class ObtenerSustanciaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Sustancia>>
    {
        public int IdSustancia { get; set; }
    }
}

