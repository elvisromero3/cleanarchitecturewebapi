using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class MonedasRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public MonedasRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        }

        [Fact]
        public async Task ObtenerMonedas()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Monedas.AddRange(
                new Moneda
                {
                    Id = 1,
                    Nombre = "Moneda 1",
                },
                new Moneda
                {
                    Id = 2,
                    Nombre = "Moneda 2",
                },
                new Moneda
                {
                    Id = 3,
                    Nombre = "Moneda 3",
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new MonedasRepository(context);
                var result = await repository.ObtenerMonedas();

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<List<Moneda>>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerMonedaPorId()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Monedas.AddRange(
                new Moneda
                {
                    Id = 1,
                    Nombre = "Moneda 1",
                },
                new Moneda
                {
                    Id = 2,
                    Nombre = "Moneda 2",
                },
                new Moneda
                {
                    Id = 3,
                    Nombre = "Moneda 3",
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new MonedasRepository(context);
                var result = await repository.ObtenerMonedaPorId(2);

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<Moneda>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerMonedaPorId_NoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Monedas.AddRange(
                new Moneda
                {
                    Id = 1,
                    Nombre = "Moneda 1",
                },
                new Moneda
                {
                    Id = 2,
                    Nombre = "Moneda 2",
                },
                new Moneda
                {
                    Id = 3,
                    Nombre = "Moneda 3",
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new MonedasRepository(context);
                var result = await repository.ObtenerMonedaPorId(4);

                //Assert
                Assert.True(result.IsError);
                Assert.Null(result.Value);
            }
        }
    }
}
