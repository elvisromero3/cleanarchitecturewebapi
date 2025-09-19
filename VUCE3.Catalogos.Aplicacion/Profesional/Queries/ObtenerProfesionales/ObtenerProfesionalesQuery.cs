using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales
{
    public class ObtenerProfesionalesQuery : IRequest<ErrorOr<List<Dominio.Entidades.Profesional>>>
    {
    }
}
