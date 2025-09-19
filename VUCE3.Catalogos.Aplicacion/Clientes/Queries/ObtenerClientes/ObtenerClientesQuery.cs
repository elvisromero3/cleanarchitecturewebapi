using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientes
{
    public class ObtenerClientesQuery : IRequest<ErrorOr<List<Cliente>>>
    {
    }
}
