using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class BarriosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public BarriosRepositoryTest()
        {
            var dbName = $"BDTestBarrios_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<BarriosRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new BarriosRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            Canton canton = new()
            {
                Id = 1,
                Nombre = "Cantón 1",
                Codigo = "01",
                IdProvincia = 1,
            };
            
            Distrito distrito = new()
            {
                Id = 1,
                Nombre = "Distrito 1",
                Codigo = "01",
                IdCanton = 1,
                Canton = canton
            };

            Barrio barrio = new()
            {
                Id = 1,
                Nombre = "Barrio 1",
                Codigo = "01",
                IdDistrito = 1,
                Distrito = distrito
            };

            context.Barrios.Add(barrio);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task ObtenerBarrios_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var barrios = repository.ObtenerBarrios();
            var result = Assert.IsType<List<Barrio>>(barrios?.Result.Value);

            //Assert
            Assert.Equal("Barrio 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerBarrioPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var barrio = repository.ObtenerBarrioPorId(1);
            var result = barrio?.Result.Value;

            //Assert            
            Assert.Equal("Barrio 1", result?.Nombre);
        }

        [Fact]
        public async Task ObtenerBarrioPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var barrio = repository.ObtenerBarrioPorId(10);
            var result = barrio?.Result.Errors;

            //Assert            
            Assert.True(barrio?.Result.IsError);
            Assert.Equal("Barrio.NoEncontrado", result?[0].Code);
            Assert.Equal("Barrio no encontrado", result?[0].Description);
        }

        [Fact]
        public async void CrearBarrio_Ok()
        {
            //Arrange
            Barrio barrio = new()
            {
                Id = 5,
                Nombre = "Barrio 1",
                Codigo = "01",
                IdDistrito = 1
            };

            //Act
            BarriosRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearBarrio(barrio);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarBarrio_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var barrio = await repository.ObtenerBarrioPorId(id);
            barrio.Value.Nombre = "Barrio 2";
            barrio.Value.Codigo = "01";

            //Act
            var barrioActualizado = await repository.ActualizarBarrio(barrio.Value, id, listaCambios);

            //Assert
            Assert.Equal("Barrio 2", barrioActualizado.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarBarrio_NoEncontrado()
        {
            var listaCambios = new List<string> { "Codigo" };
            var barrioActualizado = new Barrio
            {
                Id = 999,
                Nombre = "Barrio 1",
                Codigo = "01",
                IdDistrito = 1
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarBarrio(barrioActualizado, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Barrio.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Barrio no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarBarrio_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarBarrio(999);
            Assert.True(error.IsError);
            Assert.Equal("Barrio.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Barrio no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarBarrio_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarBarrio(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarBarrio_True()
        {
            var repository = await CreateRepositoryAsync();
            var result = await repository.ValidarBarrio(2, "01", 0);

            Assert.False(result.IsError);
            Assert.True(result.Value);
        }
    }
}
