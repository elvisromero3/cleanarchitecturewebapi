using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId
{
    public class ObtenerCultivoPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Cultivo>>
    {
        public int IdCultivo { get; set; }
    }
}
