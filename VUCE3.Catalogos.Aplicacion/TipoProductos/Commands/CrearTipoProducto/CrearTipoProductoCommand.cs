using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.CrearTipoProducto
{
    public class CrearTipoProductoCommand : IRequest<ErrorOr<Dominio.Entidades.TipoProducto>>
    {
        public Dominio.Entidades.TipoProducto TipoProducto { get; set; } = null!;
    }
}
