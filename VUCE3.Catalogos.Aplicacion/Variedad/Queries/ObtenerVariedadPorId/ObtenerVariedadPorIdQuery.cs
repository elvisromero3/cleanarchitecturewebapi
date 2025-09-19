using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedadPorId
{
    public class ObtenerVariedadPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Variedad>>
    {
        public int Id { get; set; }
    }
}
