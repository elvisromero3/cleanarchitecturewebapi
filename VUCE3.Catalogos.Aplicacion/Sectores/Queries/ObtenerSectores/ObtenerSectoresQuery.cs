using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectores
{
    public class ObtenerSectoresQuery : IRequest<ErrorOr<List<Sector>>>
    {
    
    }
}
