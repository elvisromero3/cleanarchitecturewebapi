using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId
{
    public class ObtenerPaisBloqueComercialPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.PaisBloqueComercial>>
    {
        public int IdPaisBloqueComercial { get; set; }
    }
}
