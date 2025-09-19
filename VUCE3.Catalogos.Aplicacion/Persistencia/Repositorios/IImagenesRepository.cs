using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface IImagenesRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Imagenes>>> ObtenerImagenes();

        public Task<ErrorOr<Dominio.Entidades.Imagenes>> ObtenerImagenPorId(int id);

        public Task<ErrorOr<Dominio.Entidades.Imagenes>> CrearImagen(Dominio.Entidades.Imagenes imagenes);

        public Task<ErrorOr<Dominio.Entidades.Imagenes>> ActualizarImagen(Dominio.Entidades.Imagenes imagenes, int idImagen, IEnumerable<string> changeList);

        public Task<ErrorOr<Deleted>> EliminarImagen(int id);


    }
}
