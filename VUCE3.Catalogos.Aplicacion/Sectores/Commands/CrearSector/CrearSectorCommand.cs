using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.CrearSector
{
    public class CrearSectorCommand : IRequest<ErrorOr<Sector>>
    {
        public Sector Sector { get; set; } = null!;
    }
}
