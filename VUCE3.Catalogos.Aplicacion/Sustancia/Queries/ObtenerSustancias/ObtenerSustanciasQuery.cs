using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Queries.ObtenerSustancias
{
    public class ObtenerSustanciasQuery : IRequest<ErrorOr<List<Dominio.Entidades.Sustancia>>>
    {
    }
}