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
    public class MonedasRepository : IMonedasRepository
    {
        private readonly CatalogosDbContext _context;
        public MonedasRepository(CatalogosDbContext context)
        {
            _context = context;
        }
        public async Task<ErrorOr<List<Moneda>>> ObtenerMonedas()
        {
            return await _context.Monedas
                .ToListAsync();
        }

        public async Task<ErrorOr<Moneda>> ObtenerMonedaPorId(int id)
        {
            var moneda = await _context.Monedas.FindAsync(id);

            if (moneda is null)
            {
                return ErroresMoneda.NoEncontrada;
            }

            return moneda;
        }
    }
}
