using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedas
{
    public class ObtenerMonedasQuery : IRequest<ErrorOr<List<Dominio.Entidades.Moneda>>>
    {
    }
}
