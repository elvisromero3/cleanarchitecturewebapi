using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamiliasPorId
{
    public class ObtenerFamiliaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Familia>>
    {
        public int IdFamilia { get; set; }
    }
}
