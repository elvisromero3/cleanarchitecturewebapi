using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.CrearDistrito
{
    public class CrearDistritoCommand : IRequest<ErrorOr<Distrito>>
    {
        public Distrito Distrito { get; set; } = null!;
    }
}
