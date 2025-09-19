using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EditarSustancia
{
    public class EditarSustanciaCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Sustancia, Dominio.Entidades.Sustancia>>>
    {
        public int IdSustancia { get; set; }
        public Dominio.Entidades.Sustancia Sustancia { get; set; } = null!;
        public IEnumerable<string> ListaCambios { get; set; } = null!;
    }
}