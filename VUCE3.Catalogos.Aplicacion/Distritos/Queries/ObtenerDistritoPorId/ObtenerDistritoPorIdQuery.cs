using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritoPorId
{
    public class ObtenerDistritoPorIdQuery : IRequest<ErrorOr<Distrito>>
    {
        public int Id { get; set; }
    }
}
