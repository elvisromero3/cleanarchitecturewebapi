using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IVariedadesRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Variedad>>> ObtenerVariedades();
        public Task<ErrorOr<Dominio.Entidades.Variedad>> ObtenerVariedadPorId(int id);
        public Task<ErrorOr<Dominio.Entidades.Variedad>> ObtenerVariedadPorCodigo(string codigo);
        public Task<ErrorOr<Dominio.Entidades.Variedad>> CrearVariedad(Dominio.Entidades.Variedad variedad);
        public Task<ErrorOr<Dominio.Entidades.Variedad>> EditarVariedad(Dominio.Entidades.Variedad variedad, int idVariedad, IEnumerable<string> changeList);
        public Task<ErrorOr<Deleted>> EliminarVariedad(int id);
        public Task<ErrorOr<Deleted>> EliminarVariedad(Dominio.Entidades.Variedad variedad);
        public Task<ErrorOr<bool>> ValidarVariedad(int id, string codigo, string nombre);
        public Task<ErrorOr<bool>> ExisteRelacionConCultivo(int idVariedad);
    }
}
