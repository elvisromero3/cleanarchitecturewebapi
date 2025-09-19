using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class SustanciaControladaRepository : ISustanciaControladaRepository
    {
        private readonly CatalogosDbContext _context;
        public SustanciaControladaRepository(CatalogosDbContext context) {

            _context = context;
        } 
        public async Task<ErrorOr<SustanciaControlada>> ActualizarSustanciaControlada(int idSustanciaControlada, List<string> listaCambios, SustanciaControlada sustanciaControlada)
        {
            SustanciaControlada? config = await _context.SustanciaControlada.Where(x => x.Id == idSustanciaControlada).FirstOrDefaultAsync();

            if (config is null)
            {
                return ErroresSustanciaControlada.NoEncontrada;
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, sustanciaControlada.GetType().GetProperty(change)?.GetValue(sustanciaControlada));
            }

            return sustanciaControlada;
        }

        public async Task<ErrorOr<SustanciaControlada>> CrearSustanciaControlada(SustanciaControlada sustanciaControlada)
        {
            var sustanciacontroladaCreada = await _context.SustanciaControlada.AddAsync(sustanciaControlada);
            return sustanciacontroladaCreada.Entity;
        }

        public async Task<ErrorOr<Deleted>> EliminarSustanciaControlada(int id)
        {
            var SustanciaControlada = await _context.SustanciaControlada.FindAsync(id);

            if (SustanciaControlada is null)
            {
                return ErroresSustanciaControlada.NoEncontrada;
            }

            _context.SustanciaControlada.Remove(SustanciaControlada);
            return Result.Deleted;
        }

        public async Task<ErrorOr<SustanciaControlada>> ObtenerSustanciaControladaPorId(int Id)
        {
            var SustanciaControlada = await _context.SustanciaControlada.FindAsync(Id);

            if (SustanciaControlada is null)
            {
                return ErroresSustanciaControlada.NoEncontrada;
            }

            return SustanciaControlada;
        }

        public async Task<ErrorOr<bool>> ValidarSustanciasControladas(int id, Dominio.Entidades.SustanciaControlada sustanciaControlada)
        {

            var resultados = await _context.SustanciaControlada
                .Where(sc => sc.Id != id && sc.IdFamilia == sustanciaControlada.IdFamilia)
                .ToListAsync();

            string clAra_Normalizada = Validadores.NormalizarString(sustanciaControlada.ClasificacionArancelaria);
            string clAsh_Normalizada = Validadores.NormalizarString(sustanciaControlada.ClasificacionAshrae);
            string tipoGas_Normalizado = Validadores.NormalizarString(sustanciaControlada.TipoGas);


            return resultados.Any(sc =>
                   Validadores.NormalizarString(sc.ClasificacionArancelaria) == clAra_Normalizada
                && Validadores.NormalizarString(sc.ClasificacionAshrae) == clAsh_Normalizada
                && Validadores.NormalizarString(sc.TipoGas) == tipoGas_Normalizado);
              
        }

        public async Task<ErrorOr<List<SustanciaControlada>>> ObtenerSustanciaControladas()
        {
            return await _context.SustanciaControlada.ToListAsync();
        }

        public async Task<ErrorOr<Familia>> ObtenerFamiliaSustanciaControlada(int idFamilia)
        {
            var familia = await _context.Familias.FindAsync(idFamilia);

            if (familia is null)
            {
                return ErroresSustanciaControlada.FamiliaNoEncontrada;
            }

            return familia;
        }
    }
}
