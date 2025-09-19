using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ExcepcionesMorosidadRepository : IExcepcionesMorosidadRepository
    {
        private readonly CatalogosDbContext _context;
        public ExcepcionesMorosidadRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<ExcepcionMorosidad>>> ObtenerExcepcionesMorosidad()
        {
            return await _context.ExcepcionesMorosidad.ToListAsync();
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> ObtenerExcepcionMorosidadPorId(int Id)
        {
            var excepcion = await _context.ExcepcionesMorosidad.FindAsync(Id);
            if (excepcion is null)
            {
                return ErroresExcepcionMorosidad.NoEncontrada;
            }

            return excepcion;
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> CrearExcepcionMorosidad(ExcepcionMorosidad excepcionMorosidad)
        {
            var excepcionCreada = await _context.ExcepcionesMorosidad.AddAsync(excepcionMorosidad);
            return excepcionCreada.Entity;
        }

        public async Task<ErrorOr<ExcepcionMorosidad>> ActualizarExcepcionMorosidad(int idExcepcionMorosidad, List<string> changeList, ExcepcionMorosidad excepcionMorosidad)
        {
            ExcepcionMorosidad? config = await _context.ExcepcionesMorosidad.FindAsync(idExcepcionMorosidad);
            if (config is null)
            {
                return ErroresExcepcionMorosidad.NoEncontrada;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, excepcionMorosidad.GetType().GetProperty(change)?.GetValue(excepcionMorosidad));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarExcepcionMorosidad(int id)
        {
            var excepcion = await _context.ExcepcionesMorosidad.FindAsync(id);
            if (excepcion is null)
            {
                return ErroresExcepcionMorosidad.NoEncontrada;
            }

            _context.ExcepcionesMorosidad.Remove(excepcion);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarExcepcionMorosidad(ExcepcionMorosidad excepcionMorosidad, bool excluirProgramadas)
        {
            int estadoExluido = excluirProgramadas ? ConstantesEstadosExcepcionMorosidad.PROGRAMADA : 0;
            return await _context.ExcepcionesMorosidad
                .AnyAsync(s =>
                    s.Id != excepcionMorosidad.Id &&
                    s.IdTipoTramite == excepcionMorosidad.IdTipoTramite &&
                    s.IdSubtipoTramite == excepcionMorosidad.IdSubtipoTramite &&
                    s.IdRegimen == excepcionMorosidad.IdRegimen &&
                    s.IdTipoAccion == excepcionMorosidad.IdTipoAccion &&
                    s.IdEstado != estadoExluido &&
                    s.NumeroIdentificacionEmpresa == excepcionMorosidad.NumeroIdentificacionEmpresa &&
                    (
                        (s.FechaInicio <= (excepcionMorosidad.FechaVencimiento ?? DateTime.MaxValue)) &&
                        ((s.FechaVencimiento ?? DateTime.MaxValue) >= excepcionMorosidad.FechaInicio)
                    )
                );
        }


    }
}
