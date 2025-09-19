using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class SustanciasRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public SustanciasRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        [Fact]
        public async Task ObtenerSustancias_Ok()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                context.Sustancias.Add(new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "41125",
                    ListaCaq = "Lista 2"
                });
                context.SaveChanges();
            }
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.ObtenerSustancias();
                Assert.False(result.IsError);
                Assert.Equal(2, result.Value.Count);
                Assert.IsType<List<Sustancia>>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerSustanciaPorId_Ok()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                context.Sustancias.Add(new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "41125",
                    ListaCaq = "Lista 2"
                });
                context.SaveChanges();
            }
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.ObtenerSustanciaPorId(1);
                Assert.False(result.IsError);
                Assert.Equal("Sustancia 1", result.Value.Nombre);
                Assert.IsType<Sustancia>(result.Value);
            }
        }

        // NoEncontrado
        [Fact]
        public async Task ObtenerSustanciaPorId_ErrorNoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                context.Sustancias.Add(new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "41125",
                    ListaCaq = "Lista 2"
                });
                context.SaveChanges();
            }
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.ObtenerSustanciaPorId(4);
                Assert.True(result.IsError);
                Assert.Null(result.Value);
                Assert.Equal("Sustancia.NoEncontrada", result.FirstError.Code);
                Assert.Equal("Sustancia no encontrada", result.FirstError.Description);
            }
        }

        [Fact]
        public async Task CrearSustancia_Ok()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.CrearSustancia(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                Assert.False(result.IsError);
                Assert.Equal("Sustancia 1", result.Value.Nombre);
                Assert.IsType<Sustancia>(result.Value);
            }
        }

        [Fact]
        public async Task CrearSustancia_ErrorSustanciaDuplicada()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                context.Sustancias.Add(new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "41125",
                    ListaCaq = "Lista 2"
                });
                context.SaveChanges();

                var repository = new SustanciasRepository(context);
                var result = await repository.ValidarSustancia(1, "Sustancia 1", "54698", "Lista 1");
                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task CrearSustancia_ErrorSustanciaDuplicadaSIID()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "54698",
                    ListaCaq = "Lista 1"
                });
                context.Sustancias.Add(new Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "41125",
                    ListaCaq = "Lista 2"
                });
                context.SaveChanges();

                var repository = new SustanciasRepository(context);
                var result = await repository.ValidarSustancia(0,"Sustancia 1", "54698", "Lista 1");
                Assert.True(result.Value);
            }
        }

        [Fact]
        public async Task EditarSustancia_Ok()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "455632",
                    ListaCaq = "Lista1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.ObtenerSustanciaPorId(1);

                result.Value.Id = 1;
                result.Value.Nombre = "Sustancia Editado";
                result.Value.Cas = "74589";

                var listaCambios = new List<string> { "Nombre", "Cas" };

                var resultEdit = await repository.EditarSustancia(result.Value, result.Value.Id, listaCambios);

                Assert.NotNull(resultEdit.Value);
                Assert.False(resultEdit.IsError);
                Assert.Equal("Sustancia Editado", result.Value.Nombre);
            }
        }

        [Fact] 
        public async Task EditarSustancia_ErrorNoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "455632",
                    ListaCaq = "Lista1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var result = await repository.ObtenerSustanciaPorId(1);

                result.Value.Id = 2;
                result.Value.Nombre = "Sustancia Editado";
                result.Value.Cas = "74589";

                var listaCambios = new List<string> { "Nombre", "Cas" };

                var resultEdit = await repository.EditarSustancia(result.Value, result.Value.Id, listaCambios);

                Assert.True(resultEdit.IsError);
            }
        }

        [Fact]
        public async Task EliminarSustancia_Ok()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "455632",
                    ListaCaq = "Lista1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var sustancia = await repository.EliminarSustancia(1);

                Assert.False(sustancia.IsError);
            }
        }

        [Fact]
        public async Task EliminarSustancia_ErrorNoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Sustancias.Add(new Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "455632",
                    ListaCaq = "Lista1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new SustanciasRepository(context);
                var sustancia = await repository.EliminarSustancia(2);

                Assert.True(sustancia.IsError);
            }
        }
    }
}
