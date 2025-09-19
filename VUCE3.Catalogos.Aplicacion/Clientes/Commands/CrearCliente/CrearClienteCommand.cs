using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.CrearCliente
{
    public class CrearClienteCommand : IRequest<ErrorOr<Cliente>>
    {
        public Cliente Cliente { get; set; } = null!;
    }
}
