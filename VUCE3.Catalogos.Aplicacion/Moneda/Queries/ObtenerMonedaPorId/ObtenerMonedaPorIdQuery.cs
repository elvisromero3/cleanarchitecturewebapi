using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedaPorId
{
    public class ObtenerMonedaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Moneda>>
    {
        public int IdMoneda {  get; set; }
    }
}
