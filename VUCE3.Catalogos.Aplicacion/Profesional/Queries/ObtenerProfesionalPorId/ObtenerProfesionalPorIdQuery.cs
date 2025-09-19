using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionalPorId
{
    public class ObtenerProfesionalPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Profesional>>
    {
        public int Id { get; set; }
    }
}
