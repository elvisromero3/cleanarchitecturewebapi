using ErrorOr;
using MediatR;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.CrearProfesional
{
    public class CrearProfesionalCommand : IRequest<ErrorOr<Dominio.Entidades.Profesional>>
    {
        public Dominio.Entidades.Profesional Profesional { get; set; } = null!;
    }
}
