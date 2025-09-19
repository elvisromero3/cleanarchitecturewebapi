using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class DistritosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public DistritosRepositoryTest()
        {
            var dbName = $"BDTestDistritos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<DistritosRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new DistritosRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var barrios = new List<Barrio>(){new  Barrio()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Barrio 1",
                    IdDistrito = 1
                }
            };
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

            Distrito distrito2 = new()
            {
                Id = 2,
                Nombre = "Distrito 1",
                Codigo = "01",
                IdCanton = 1,
                Canton = canton,
                Barrios = barrios
            };
            context.Distritos.Add(distrito);
            context.Distritos.Add(distrito2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task ObtenerDistritos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var distritos = repository.ObtenerDistritos();
            var result = Assert.IsType<List<Distrito>>(distritos?.Result.Value);

            //Assert
            Assert.Equal("Distrito 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerDistritoPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var distrito = repository.ObtenerDistritoPorId(1);
            var result = distrito?.Result.Value;

            //Assert            
            Assert.Equal("Distrito 1", result?.Nombre);
        }

        [Fact]
        public async Task ObtenerDistritoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var distrito = repository.ObtenerDistritoPorId(10);
            var result = distrito?.Result.Errors;

            //Assert            
            Assert.True(distrito?.Result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result?[0].Code);
            Assert.Equal("Distrito no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarDistrito_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var distrito = await repository.ObtenerDistritoPorId(id);
            distrito.Value.Nombre = "Distrito 2";
            distrito.Value.Codigo = "01";

            //Act
            var distritoActualizado = await repository.ActualizarDistrito(distrito.Value, id, listaCambios );

            //Assert
            Assert.Equal("Distrito 2", distritoActualizado.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarDistrito_NoEncontrado()
        {
            var listaCambios = new List<string> { "Codigo" };
            var distritoActualizado = new Distrito
            {
                Id = 999,
                Nombre = "Distrito 1",
                Codigo = "01",
                IdCanton =1
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarDistrito(distritoActualizado, 999, listaCambios );
            Assert.True(error.IsError);
            Assert.Equal("Distrito.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Distrito no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearDistrito_Ok()
        {
            //Arrange
            Distrito distrito = new()
            {
                Id = 5,
                Nombre = "Distrito 1",
                Codigo = "01",
                IdCanton = 1
            };

            //Act
            DistritosRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearDistrito(distrito);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarDistrito_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarDistrito(999);
            Assert.True(error.IsError);
            Assert.Equal("Distrito.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Distrito no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarDistrito_BarrioRelacionado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarDistrito(2);
            Assert.True(error.IsError);
            Assert.Equal("Distrito.BarriosRelacionados", error.Errors[0].Code);
            Assert.Equal("Existe distrito con barrios relacionados", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarDistrito_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarDistrito(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarDistrito_True()
        {
            var repository = await CreateRepositoryAsync();
            var result = await repository.ValidarDistrito(2, "01", 0);

            Assert.False(result.IsError);
            Assert.True (result.Value);
        }
    }
}
