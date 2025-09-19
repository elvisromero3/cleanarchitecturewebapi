using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IBloqueComercialRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.BloqueComercial>>> ObtenerBloquesComerciales();
        public Task<ErrorOr<Dominio.Entidades.BloqueComercial>> ObtenerBloqueComercialPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.BloqueComercial>> CrearBloqueComercial(Dominio.Entidades.BloqueComercial bloquecomercial);

        public Task<ErrorOr<Dominio.Entidades.BloqueComercial>> ActualizarBloqueComercial(int idBloqueComercial, List<string> listaCambios, Dominio.Entidades.BloqueComercial bloquecomercial);

        public Task<ErrorOr<Deleted>> EliminarBloqueComercial(int id);

        public Task<ErrorOr<bool>> ValidarBloqueComercial(int idBloqueComercial, string nombre);
    }
}
