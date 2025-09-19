using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId
{
    public class ObtenerTarifaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Tarifa>>
    {
        public int IdTarifa { get; set; }
    }
}
