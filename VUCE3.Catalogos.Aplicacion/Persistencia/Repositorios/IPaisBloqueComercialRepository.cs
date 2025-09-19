using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IPaisBloqueComercialRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.PaisBloqueComercial>>> ObtenerPaisBloqueComercial();
        public Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> ObtenerPaisBloqueComercialPorId(int Id);

        public Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> CrearPaisBloqueComercial(Dominio.Entidades.PaisBloqueComercial paisBloqueComercial);

        public Task<ErrorOr<Dominio.Entidades.PaisBloqueComercial>> ActualizarPaisBloqueComercial(int idPaisBloqueComercial, List<string> listaCambios, Dominio.Entidades.PaisBloqueComercial paisBloqueComercial);

        public Task<ErrorOr<Deleted>> EliminarPaisBloqueComercial(int id);

        public Task<ErrorOr<bool>> ValidarPaisBloqueComercial(int idBloqueComercial, int idPais);
    }
}
