using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId
{
    public class ObtenerPaisPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Pais>>
    {
        public int Id { get; set; }
    }
}
