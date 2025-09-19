using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class TarifasRepository : ITarifasRepository
    {
        private readonly CatalogosDbContext _context;
        public TarifasRepository(CatalogosDbContext context)
        {

            _context = context;
        }
        public async Task<ErrorOr<List<Tarifa>>> ObtenerTarifas()
        {
            return await _context.Tarifas.ToListAsync();
        }

        public async Task<ErrorOr<Tarifa>> ObtenerTarifaPorId(int id)
        {
            var tarifa = await _context.Tarifas.FindAsync(id);

            if (tarifa is null)
            {
                return ErroresTarifa.NoEncontrada;
            }

            return tarifa;
        }
    }
}
