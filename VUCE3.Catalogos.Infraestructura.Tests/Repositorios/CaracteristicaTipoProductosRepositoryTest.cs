using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CaracteristicaTipoProductoRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public CaracteristicaTipoProductoRepositoryTest()
        {
            var dbName = $"BDTestDCaracteristicaTipoProductos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<CaracteristicaTipoProductoRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new CaracteristicaTipoProductoRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            CaracteristicaTipoProducto CaracteristicaTipoProducto = new()
            {
                Id = 1,
                IdTipoProducto = 1,
                IdCaracteristica = 1,
                Caracteristica = new Caracteristica
                {
                    Id = 1,
                    Nombre = "Caracteristica 1",
                    IdInstitucion = 5
                },
                TipoProducto = new TipoProducto
                {
                    Id = 1,
                    IdCategoria = 1,
                    IdInstitucion = 5,
                    Tipo = "Tipo Producto 1"
                }
            };

            context.CaracteristicaTipoProductos.Add(CaracteristicaTipoProducto);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerCaracteristicaTipoProductos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var CaracteristicaTipoProductos = repository.ObtenerCaracteristicaTipoProductos();
            var result = Assert.IsType<List<CaracteristicaTipoProducto>>(CaracteristicaTipoProductos?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].Id);
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var CaracteristicaTipoProductos = repository.ObtenerCaracteristicaTipoProductoPorId(1);
            var result = CaracteristicaTipoProductos?.Result.Value;

            //Assert            
            Assert.Equal(1, result?.Id);
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var CaracteristicaTipoProducto = repository.ObtenerCaracteristicaTipoProductoPorId(10);
            var result = CaracteristicaTipoProducto?.Result.Errors;

            //Assert            
            Assert.True(CaracteristicaTipoProducto?.Result.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", result?[0].Code);
            Assert.Equal("Característica tipo producto no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarCaracteristicaTipoProducto_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "IdTipoProducto" };
            var repository = await CreateRepositoryAsync();

            var CaracteristicaTipoProducto = await repository.ObtenerCaracteristicaTipoProductoPorId(id);
            CaracteristicaTipoProducto.Value.IdTipoProducto = 1;

            //Act
            var CaracteristicaTipoProductoActualizada = await repository.ActualizarCaracteristicaTipoProducto(1, listaCambios, CaracteristicaTipoProducto.Value);

            //Assert
            Assert.Equal(1, CaracteristicaTipoProductoActualizada.Value.IdTipoProducto);
        }
        [Fact]
        public async Task ActualizarCaracteristicaTipoProducto_NoEncontrada()
        {
            var listaCambios = new List<string> { "Nombre" };
            var CaracteristicaTipoProductoActualizada = new CaracteristicaTipoProducto { Id = 999, IdTipoProducto = 1, IdCaracteristica = 1 };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCaracteristicaTipoProducto(10, listaCambios, CaracteristicaTipoProductoActualizada);
            Assert.True(error.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Característica tipo producto no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearCaracteristicaTipoProductos_Ok()
        {
            //Arrange
            CaracteristicaTipoProducto CaracteristicaTipoProducto = new()
            {
                Id = 5,
                IdCaracteristica = 1,
                IdTipoProducto = 1
            };


            //Act
            CaracteristicaTipoProductoRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearCaracteristicaTipoProducto(CaracteristicaTipoProducto);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCaracteristicaTipoProducto(999);
            Assert.True(error.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Característica tipo producto no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCaracteristicaTipoProducto(1);

            Assert.False(error.IsError);
        }
        [Fact]
        public async Task ValidarCaracteristicaTipoProducto_False()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var CaracteristicaTipoProducto = await repository.ObtenerCaracteristicaTipoProductoPorId(1);
            var error = await repository.ValidarCaracteristicaTipoProducto(1, 2);
            Assert.False(error.Value);
        }

        [Fact]
        public async Task ValidarCaracteristicaTipoProducto_True()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var CaracteristicaTipoProducto = await repository.ObtenerCaracteristicaTipoProductoPorId(1);
            var error = await repository.ValidarCaracteristicaTipoProducto(1, 1);
            Assert.True(error.Value);
        }

        [Fact]
        public async Task VerificaExistenciaPorTipoProducto_True()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var existe = await repository.VerificaExistenciaPorTipoProducto(1);
            
            Assert.True(existe.Value);
        }
    }
}
