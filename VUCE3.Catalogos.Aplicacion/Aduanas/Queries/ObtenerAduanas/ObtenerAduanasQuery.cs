using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanas
{
    public class ObtenerAduanasQuery : IRequest<ErrorOr<List<Aduana>>>
    {

    }
}
