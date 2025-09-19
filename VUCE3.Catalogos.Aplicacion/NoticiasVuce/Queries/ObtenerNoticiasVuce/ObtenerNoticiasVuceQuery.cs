using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVuce
{
    public class ObtenerNoticiasVuceQuery : IRequest<ErrorOr<List<Dominio.Entidades.NoticiasVuce>>>
    {
    }
}
