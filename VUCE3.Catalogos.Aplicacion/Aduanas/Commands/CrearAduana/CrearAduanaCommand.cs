using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.CrearAduana
{
    public class CrearAduanaCommand : IRequest<ErrorOr<Aduana>>
    {
        public Aduana Aduana { get; set; } = null!;
    }
}
