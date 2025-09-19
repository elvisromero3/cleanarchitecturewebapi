using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicaPorId
{
    public class ObtenerCaracteristicaPorIdQuery : IRequest<ErrorOr<Caracteristica>>
    {
        public int Id { get; set; }
    }
}
