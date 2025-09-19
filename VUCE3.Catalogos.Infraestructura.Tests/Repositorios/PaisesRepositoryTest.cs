using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class PaisesRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public PaisesRepositoryTest()
        {
            var dbName = $"BDTestPaises_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;        
        }        

        private async Task<PaisesRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new PaisesRepository(context);
        }

        [Fact]
        public async Task ObtenerPaises_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paises = repository.ObtenerPaises();
            var result = Assert.IsType<List<Pais>>(paises?.Result.Value);

            //Assert
            Assert.Equal("Venezuela", result[0].Nombre);            
        }

        [Fact]
        public async Task ObtenerPaisPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paises = repository.ObtenerPaisPorId(1);
            var result = paises?.Result.Value;

            //Assert            
            Assert.Equal("Venezuela", result?.Nombre);            
        }

        [Fact]
        public async Task ObtenerPaisPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paises = repository.ObtenerPaisPorId(10);
            var result = paises?.Result.Errors;

            //Assert            
            Assert.True(paises?.Result.IsError);
            Assert.Equal("Pais.NoEncontrado", result?[0].Code);
            Assert.Equal("País no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarPais_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre pais cambio" };
            var repository = await CreateRepositoryAsync();

            var pais = await repository.ObtenerPaisPorId(id);
            pais.Value.Nombre = "Nombre pais";

            //Act
            var paisActualizada = await repository.ActualizarPais(pais.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Nombre pais", paisActualizada.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarPais_NoEncontrada()
        {
            var listaCambios = new List<string> { "Venezuela" };
            var paisActualizada = new Pais { Id = 999, Nombre = "Nombre pais" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarPais(paisActualizada, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Pais.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("País no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task CrearPais_Ok()
        {
            var pais = new Pais
            {
                Id = 14,
                Nombre = "España",
                CodigoC3 = "ESP",
                CodigoA2 = "ES",
                CodigoNumerico = "123"
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var paisNueva = await repository.CrearPais(pais);

            //Assert
            var result = Assert.IsType<ErrorOr<Pais>>(paisNueva);
            Assert.Equal(14, result.Value.Id);
            Assert.Equal("España", result.Value.Nombre);            
        }

        [Fact]
        public async Task EliminarPais_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarPais(999);
            Assert.True(error.IsError);
            Assert.Equal("Pais.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("País no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarPais_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarPais(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task EliminarPais_PerteneceBloqueComercial()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarPais(2);

            Assert.True(error.IsError);
        }

        [Fact]
        public async Task EliminarPais_PerteneceRegistro()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarPais(160);

            Assert.True(error.IsError);
        }

        [Fact]
        public async Task PaisesDuplicada_DevuelveTrue()
        {

            // Arrange
            var pais = new Pais
            {
                Id = 14,
                Nombre = "España",
                CodigoC3 = "ESP",
                CodigoA2 = "ES",
                CodigoNumerico = "123"
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task PaisesDuplicada_DevuelveFalse()
        {

            // Arrange
            var pais = new Pais
            {
                Id = 14,
                Nombre = "Reino Unido",
                CodigoC3 = "GBR",
                CodigoA2 = "GB",
                CodigoNumerico = "120"
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task PaisesDuplicada_MismoDevuelveFalse()
        {

            // Arrange
            var pais = new Pais
            {
                Id = 2,
                Nombre = "España",
                CodigoC3 = "ESP",
                CodigoA2 = "ES",
                CodigoNumerico = "130"
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3);
            Assert.Null(result.Value);
        }


        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            var pais1 = new Pais
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais2 = new Pais
            {
                Id = 2,
                Nombre = "España",
                CodigoC3 = "ESP",
                CodigoA2 = "ES",
                CodigoNumerico = "124"
            };

            var pais3 = new Pais
            {
                Id = 3,
                Nombre = "Alemania",
                CodigoC3 = "ALE",
                CodigoA2 = "AL",
                CodigoNumerico = "125"
            };

            var bloqueComercial1 = new BloqueComercial
            {
                Id = 1,
                Nombre = "Mercosur",
               
            };
            var paisBloqueComercial1 = new PaisBloqueComercial
            {
                Id = 1,
                IdPais = 2,
                IdBloqueComercial = 1
            };

            context.Paises.Add(pais1);
            context.Paises.Add(pais2);
            context.Paises.Add(pais3);
            context.BloquesComerciales.Add(bloqueComercial1);
            context.PaisBloqueComercial.Add(paisBloqueComercial1);

            await context.SaveChangesAsync();
        }

    }
}
