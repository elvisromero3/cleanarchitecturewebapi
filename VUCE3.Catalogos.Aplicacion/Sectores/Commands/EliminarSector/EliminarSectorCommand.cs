using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSector
{
    public class EliminarSectorCommand : IRequest<ErrorOr<Sector>>
    {
        public int Id { get; set; }
    }
}
