using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class AduanasRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public AduanasRepositoryTest()
        {
            var dbName = $"BDTestAduanas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<AduanasRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new AduanasRepository(context);
        }

        [Fact]
        public async Task ObtenerAduanas_OK()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var aduanas = repository.ObtenerAduanas();
            var result = Assert.IsType<List<Aduana>>(aduanas?.Result.Value);

            //Assert           
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Aduana 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerAduanaPorId_OK()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var result = repository.ObtenerAduanaPorId(2);

            //Assert
            Assert.False(result?.Result.IsError);
            Assert.Equal(2, result?.Result.Value.Id);
            Assert.Equal("Aduana 2", result?.Result.Value.Nombre);
        }

        [Fact]
        public async Task ObtenerAduanaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act            
            var result = await repository.ObtenerAduanaPorId(3);

            //Assert

            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearAduana_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            var aduana10 = new Aduana
            {
                Id = 10,
                Nombre = "Aduana 10"
            };

            //Act
            var aduanaNueva = await repository.CrearAduana(aduana10);

            //Assert
            var result = Assert.IsType<ErrorOr<Aduana>>(aduanaNueva);
            Assert.Equal(10, result.Value.Id);
            Assert.Equal("Aduana 10", result.Value.Nombre);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var aduana1 = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var aduana2 = new Aduana
            {
                Id = 2,
                Nombre = "Aduana 2"
            };

            context.Aduanas.Add(aduana1);
            context.Aduanas.Add(aduana2);
            
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task ActualizarAduana_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var aduana = await repository.ObtenerAduanaPorId(id);
            aduana.Value.Nombre = "Nombre b";

            //Act
            var aduanaActualizada = await repository.ActualizarAduana(aduana.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Nombre b", aduanaActualizada.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarAduana_NoEncontrada()
        {
            var listaCambios = new List<string> { "Aduana" };
            var aduanaActualizada = new Aduana { Id = 999, Nombre = "Aduana" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarAduana(aduanaActualizada, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Aduana.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Aduana no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarAduana_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarAduana(999);
            Assert.True(error.IsError);
            Assert.Equal("Aduana.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Aduana no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarAduana_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarAduana(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarAduana_True()
        {
            var repository = await CreateRepositoryAsync();
            var aduana = new Aduana { Id = 1, Nombre = "Aduana 2" };

            var result = await repository.ValidarAduana(aduana);

            Assert.False(result.IsError);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ValidarAduana_MismaFalse()
        {
            var repository = await CreateRepositoryAsync();
            var aduana = new Aduana { Id = 1, Nombre = "Aduana 1" };

            var result = await repository.ValidarAduana(aduana);

            Assert.False(result.IsError);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ValidarAduana_False()
        {
            var repository = await CreateRepositoryAsync();
            var aduana = new Aduana { Id = 1, Nombre = "Aduana mod" };

            var result = await repository.ValidarAduana(aduana);

            Assert.False(result.IsError);
            Assert.False(result.Value);
        }
    }
}
