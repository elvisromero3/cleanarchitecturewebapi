using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritos
{
    public class ObtenerDistritosQuery : IRequest<ErrorOr<List<Distrito>>>
    {
    }
}
