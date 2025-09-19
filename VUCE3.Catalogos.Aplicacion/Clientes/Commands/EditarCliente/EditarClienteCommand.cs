using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.EditarCliente
{
    public class EditarClienteCommand : IRequest<ErrorOr<Tuple<Cliente, Cliente>>>
    {
        public Cliente Cliente { get; set; }
        public int IdCliente { get; set; }
        public IEnumerable<string> ListaCambios { get; set; }

        public EditarClienteCommand(Cliente Cliente, int idCliente, IEnumerable<string> listaCambio)
        {
            this.Cliente = Cliente;
            this.IdCliente = idCliente;
            this.ListaCambios = listaCambio;
        }
    }
}
