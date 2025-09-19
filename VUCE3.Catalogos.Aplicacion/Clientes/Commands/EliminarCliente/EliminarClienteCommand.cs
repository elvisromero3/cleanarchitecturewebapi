using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarCliente
{
    public class EliminarClienteCommand : IRequest<ErrorOr<Cliente>>
    {
        public int IdCliente { get; set; }
    }
}
