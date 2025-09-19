using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductosPorId
{
    public class ObtenerCaracteristicaTipoProductoPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.CaracteristicaTipoProducto>>
    {
        public int IdCaracteristicaTipoProducto { get; set; }
    }
}
