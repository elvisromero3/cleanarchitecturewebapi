using ErrorOr;


namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICultivosRepository
    {
        Task<ErrorOr<List<Dominio.Entidades.Cultivo>>> ObtenerCultivos();
        Task<ErrorOr<Dominio.Entidades.Cultivo>> ObtenerCultivoPorId(int id);
        Task<ErrorOr<Dominio.Entidades.Cultivo>> CrearCultivo(Dominio.Entidades.Cultivo cultivo);
        Task<ErrorOr<Dominio.Entidades.Cultivo>> EditarCultivo(Dominio.Entidades.Cultivo cultivo, int idCultivo, IEnumerable<string> changeList);
        Task<ErrorOr<Deleted>> EliminarCultivo(int id);
        Task<ErrorOr<bool>> ValidarCultivo(int idCultivo, string codigo, string nombre, string nombreCientifico);
    }
}
