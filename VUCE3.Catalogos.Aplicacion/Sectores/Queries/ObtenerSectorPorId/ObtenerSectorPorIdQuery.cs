using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectorPorId
{
    public class ObtenerSectorPorIdQuery : IRequest<ErrorOr<Sector>>
    {
        public int IdSector { get; set; }
    }
}
