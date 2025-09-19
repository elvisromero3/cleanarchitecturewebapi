using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CantonRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public CantonRepositoryTest()
        {
            var dbName = $"BDTestDCantones_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<CantonesRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new CantonesRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var distrito = new List<Distrito>(){new  Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "distrito 1",
                    IdCanton = 2
                }
            };

            Canton canton = new()
            {
                Id = 1,
                Nombre = "Canton 1",
                Codigo = "01"
            };
            Canton canton2 = new()
            {
                Id = 2,
                Nombre = "Canton 2",
                Codigo = "02",
                Distritos =distrito
            };
            context.Cantones.Add(canton);
            context.Cantones.Add(canton2);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerCantons_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var cantons = repository.ObtenerCantones();
            var result = Assert.IsType<List<Canton>>(cantons?.Result.Value);

            //Assert
            Assert.Equal("Canton 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerCantonsPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var cantons = repository.ObtenerCantonesPorId(1);
            var result = cantons?.Result.Value;

            //Assert            
            Assert.Equal("Canton 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerCantonPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var canton = repository.ObtenerCantonesPorId(10);
            var result = canton?.Result.Errors;

            //Assert            
            Assert.True(canton?.Result.IsError);
            Assert.Equal("Canton.NoEncontrado", result?[0].Code);
            Assert.Equal("Cantón no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarCanton_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var canton = await repository.ObtenerCantonesPorId(id);
            canton.Value.Nombre = "Canton 2";
            canton.Value.Codigo = "02";

            //Act
            var cantonActualizada = await repository.ActualizarCantones(1, listaCambios, canton.Value);

            //Assert
            Assert.Equal("Canton 2", cantonActualizada.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarCanton_NoEncontrada()
        {
            var listaCambios = new List<string> { "Codigo" };
            var cantonActualizada = new Canton
            {
                Id = 999,
                Nombre = "Nombre Canton",
                Codigo = "01"
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCantones(10, listaCambios, cantonActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Canton.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Cantón no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearCanton_Ok()
        {
            //Arrange
            Canton canton = new()
            {
                Id = 5,
                Nombre = "Canton 1",
                Codigo = "01"
            };


            //Act
            CantonesRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearCantones(canton);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarCanton_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCantones(999);
            Assert.True(error.IsError);
            Assert.Equal("Canton.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Cantón no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCanton_ErrorDistritosRelacionados()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCantones(2);

            Assert.True(error.IsError);
            Assert.Equal("Canton.DistritosRelacionados", error.Errors[0].Code);
            Assert.Equal("Existe cantón con distritos relacionados", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCanton_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCantones(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task CantonDuplicado_DevuelveTrue()
        {

            // Arrange
            var canton = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1",
                Codigo = "01",
                IdProvincia = 1
            };

            var canton2 = new Canton()
            {
                Id = 3,
                Nombre = "Canton 1",
                Codigo = "01",
                IdProvincia = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cantones.Add(canton);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CantonesRepository(context);
                var result = await repository.ValidarCantones(canton2.Id, canton2.Codigo,0);

                Assert.True(result.Value);
            }
        }

        [Fact]
        public async Task CantonDuplicado_DevuelveFalse()
        {
            var distrito = new List<Distrito>(){new  Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "distrito 1",
                    IdCanton = 8
                }
            };

            // Arrange
            var canton = new Canton()
            {
                Id = 5,
                Nombre = "Canton 2",
                Codigo = "01"
            };

            // Arrange
            var canton2 = new Canton()
            {
                Id = 8,
                Nombre = "Canton 3",
                Codigo = "02",
                Distritos = distrito
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cantones.Add(canton);
                context.Cantones.Add(canton2);               
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CantonesRepository(context);
                var result = await repository.ValidarCantones(canton2.Id, canton2.Codigo, 0);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task CantonDuplicada_DevuelveError()
        {
            // Arrange            

            var cantonCrear = new Canton()
            {
                Id = 1,
                Nombre = "Canton 1",
                Codigo = "01",
                IdProvincia = 1
            };

            // Arrange
            var cantonExiste = new Canton
            {
                Nombre = "Canton 1",
                Codigo = "01",
                IdProvincia = 1

            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cantones.Add(cantonCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CantonesRepository(context);
                var result = await repository.ValidarCantones(cantonExiste.Id, cantonExiste.Codigo, 0);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
