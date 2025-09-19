using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos
{
    public class ObtenerCultivosQuery : IRequest<ErrorOr<List<Dominio.Entidades.Cultivo>>>
    {
    }
}
