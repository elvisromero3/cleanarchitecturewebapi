using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EditarCultivo
{
    public class EditarCultivoCommand : IRequest<ErrorOr<Tuple<Dominio.Entidades.Cultivo, Dominio.Entidades.Cultivo>>>
    {
        public int IdCultivo { get; set; }
        public Dominio.Entidades.Cultivo Cultivo { get; set; } = null!;
        public IEnumerable<string> ListaCambios { get; set; } = null!;
    }
}
