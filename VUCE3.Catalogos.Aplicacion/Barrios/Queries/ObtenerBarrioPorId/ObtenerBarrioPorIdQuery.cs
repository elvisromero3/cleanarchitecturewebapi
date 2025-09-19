using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrioPorId
{
    public class ObtenerBarrioPorIdQuery : IRequest<ErrorOr<Barrio>>
    {
        public int Id { get; set; }
    }
}
