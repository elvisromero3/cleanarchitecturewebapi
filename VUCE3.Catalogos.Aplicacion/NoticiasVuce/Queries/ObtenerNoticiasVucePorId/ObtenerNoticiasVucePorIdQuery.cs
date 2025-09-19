using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVucePorId
{
    public class ObtenerNoticiasVucePorIdQuery : IRequest<ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        public int Id { get; set; }
    }
}
