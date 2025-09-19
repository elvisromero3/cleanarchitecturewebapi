using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientePorId
{
    public class ObtenerClientePorIdQuery : IRequest<ErrorOr<Cliente>>
    {
        public int Id { get; set; }
    }
}
