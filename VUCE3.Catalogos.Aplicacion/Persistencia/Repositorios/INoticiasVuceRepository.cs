using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface INoticiasVuceRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.NoticiasVuce>>> ObtenerNoticiasVuce();
        public Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> ObtenerNoticiasVucePorId(int id);
        public Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> CrearNoticiasVuce(Dominio.Entidades.NoticiasVuce noticiasVuce);
        public Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> ActualizarNoticiasVuce(Dominio.Entidades.NoticiasVuce noticiasVuce, int idNoticiasVuce, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarNoticiasVuce(int id);
        public Task<ErrorOr<bool>> ValidarNoticiasVuce(string titulo, string texto, string? enlace, string tituloIngles, string textoIngles);

    }
}
