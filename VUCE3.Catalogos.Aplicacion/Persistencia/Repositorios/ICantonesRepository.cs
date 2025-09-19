using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICantonesRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Canton>>> ObtenerCantones();
        public Task<ErrorOr<Dominio.Entidades.Canton>> ObtenerCantonesPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.Canton>> CrearCantones(Dominio.Entidades.Canton cantones);

        public Task<ErrorOr<Dominio.Entidades.Canton>> ActualizarCantones(int idCantones, List<string> listaCambios, Dominio.Entidades.Canton cantones);

        public Task<ErrorOr<Deleted>> EliminarCantones(int id);

        public Task<ErrorOr<bool>> ValidarCantones(int idCanton, string codigo, int idProvincia);
    }
}
