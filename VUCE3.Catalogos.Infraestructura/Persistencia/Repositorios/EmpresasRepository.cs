using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class EmpresasRepository : IEmpresasRepository
    {
        private readonly CatalogosDbContext _context;

        public EmpresasRepository(CatalogosDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<List<Empresa>>> ObtenerEmpresas()
        {
            return await _context.Empresas.ToListAsync();
        }

        public async Task<ErrorOr<Empresa>> ObtenerEmpresaPorId(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa is null)
            {
                return ErroresEmpresas.NoEncontrado;
            }

            return empresa;
        }        

        public async Task<ErrorOr<Empresa>> CrearEmpresa(Empresa empresa)
        {
            var empresaCreado = await _context.Empresas.AddAsync(empresa);
            return empresaCreado.Entity;            
        }

        public async Task<ErrorOr<Empresa>> ActualizarEmpresa(Empresa empresa, int idEmpresa, IEnumerable<string> changeList)
        {
            Empresa? config = await _context.Empresas.FindAsync(idEmpresa);

            if (config is null)
            {
                return ErroresEmpresas.NoEncontrado;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, empresa.GetType().GetProperty(change)?.GetValue(empresa));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa is null)
            {
                return ErroresEmpresas.NoEncontrado;
            }

            _context.Empresas.Remove(empresa);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarEmpresa(int idEmpresa, char idTipoIdentificacion, string numeroIdentificacion, int idProfesional)
        {
            return await _context.Empresas
                .AnyAsync(e => e.Id != idEmpresa &&  e.NumeroIdentificacion == numeroIdentificacion 
                && e.IdProfesional == idProfesional && e.IdTipoIdentificacion == idTipoIdentificacion);
        }

        public async Task<ErrorOr<List<Empresa>>> ObtenerEmpresasPorIdProfesional(int id)
        {
            return await _context.Empresas.Where(x => x.IdProfesional == id) .ToListAsync();
        }
    }
}
