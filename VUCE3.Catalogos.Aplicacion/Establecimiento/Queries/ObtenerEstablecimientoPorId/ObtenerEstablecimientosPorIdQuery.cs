using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientoPorId
{
    public class ObtenerEstablecimientosPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Establecimiento>>
    {
        public int IdEstablecimiento { get; set; }
    }
}
