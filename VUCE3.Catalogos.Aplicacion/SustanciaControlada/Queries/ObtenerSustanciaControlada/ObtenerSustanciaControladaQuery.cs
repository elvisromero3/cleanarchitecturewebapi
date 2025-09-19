using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladas
{
    public class ObtenerSustanciaControladasQuery : IRequest<ErrorOr<List<Dominio.Entidades.SustanciaControlada>>>
    {
    }
}
