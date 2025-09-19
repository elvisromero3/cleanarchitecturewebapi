using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial
{
    public class ObtenerPaisBloqueComercialQuery : IRequest<ErrorOr<List<Dominio.Entidades.PaisBloqueComercial>>>
    {
    }
}
