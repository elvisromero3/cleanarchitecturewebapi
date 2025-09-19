using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICasaRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Casa>>> ObtenerCasas();
        public Task<ErrorOr<Dominio.Entidades.Casa>> ObtenerCasaPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Casa>> CrearCasa(Dominio.Entidades.Casa casa);

        public Task<ErrorOr<Dominio.Entidades.Casa>> ActualizarCasa(int idCasa, List<string> listaCambios, Dominio.Entidades.Casa casa);

        public Task<ErrorOr<Deleted>> EliminarCasa(int id);

        public Task<ErrorOr<bool>> ValidarCasa(int idCasa, string codigo, string nombre);
    }
}
