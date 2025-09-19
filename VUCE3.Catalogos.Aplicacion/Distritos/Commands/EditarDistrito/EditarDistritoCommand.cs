using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EditarDistrito
{
    public class EditarDistritoCommand : IRequest<ErrorOr<Tuple<Distrito, Distrito>>>
    {
        public Distrito Distrito { get; set; } = null!;
        public required int IdDistrito { get; set; }
        public required List<string> ListaCambios { get; set; }
    }
}
