using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantones
{
    public class ObtenerCantonesQuery : IRequest<ErrorOr<List<Dominio.Entidades.Canton>>>
    {
    }
}
