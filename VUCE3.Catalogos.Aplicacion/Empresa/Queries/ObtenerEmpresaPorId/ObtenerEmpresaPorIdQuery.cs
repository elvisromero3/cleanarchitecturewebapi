using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresaPorId
{
    public class ObtenerEmpresaPorIdQuery : IRequest<ErrorOr<Dominio.Entidades.Empresa>>
    {
        public int Id { get; set; }
    }
}
