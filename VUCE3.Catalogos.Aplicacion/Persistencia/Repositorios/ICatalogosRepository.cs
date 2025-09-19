using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios
{
    public interface ICatalogosRepository
    {
        public Task<ErrorOr<List<Dominio.Entidades.Catalogo>>> ObtenerCatalogos();

        public Task<ErrorOr<List<Dominio.Entidades.Catalogo>>> ObtenerCatalogosPorIdInstitucion(int idInstitucion);

        public Task<ErrorOr<Dominio.Entidades.Catalogo>> ObtenerCatalogoPorId(int id);

        public Task<ErrorOr<Dominio.Entidades.Catalogo>> CrearCatalogo(Dominio.Entidades.Catalogo catalogo);

        public Task<ErrorOr<Updated>> ActualizarCatalogo(Dominio.Entidades.Catalogo catalogo);

        public Task<ErrorOr<Deleted>> EliminarCatalogo(int id);


    }
}
