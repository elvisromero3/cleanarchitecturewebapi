using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class ProductoRequisitoRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public ProductoRequisitoRepositoryTest()
        {
            var dbName = $"BDTestDProductoRequisitos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<ProductoRequisitoRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new ProductoRequisitoRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            ProductoRequisito ProductoRequisito = new()
            {
                Id =1,
                IdRequisito=1,
                IdTipoProducto=1,
                TipoProducto = new TipoProducto() { IdCategoria = 1, Id = 1, Tipo = "TipoProducto" },
            };

            context.ProductoRequisitos.Add(ProductoRequisito);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerProductoRequisitos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var ProductoRequisitos = repository.ObtenerProductoRequisitos();
            var result = Assert.IsType<List<ProductoRequisito>>(ProductoRequisitos?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].Id);
        }
        [Fact]
        public async Task ObtenerProductoRequisitoPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var ProductoRequisitos = repository.ObtenerProductoRequisitoPorId(1);
            var result = ProductoRequisitos?.Result.Value;

            //Assert            
            Assert.Equal(1, result?.Id);
        }
        [Fact]
        public async Task ObtenerProductoRequisitoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var ProductoRequisito = repository.ObtenerProductoRequisitoPorId(10);
            var result = ProductoRequisito?.Result.Errors;

            //Assert            
            Assert.True(ProductoRequisito?.Result.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", result?[0].Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarProductoRequisito_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "IdTipoProducto" };
            var repository = await CreateRepositoryAsync();

            var ProductoRequisito = await repository.ObtenerProductoRequisitoPorId(id);
            ProductoRequisito.Value.IdTipoProducto = 1;

            //Act
            var ProductoRequisitoActualizada = await repository.ActualizarProductoRequisito(1, listaCambios, ProductoRequisito.Value);

            //Assert
            Assert.Equal(1, ProductoRequisitoActualizada.Value.IdTipoProducto);
        }
        [Fact]
        public async Task ActualizarProductoRequisito_NoEncontrada()
        {
            var listaCambios = new List<string> { "IdTipoProducto" };
            var ProductoRequisitoActualizada = new  ProductoRequisito { Id = 999, IdTipoProducto=1,IdRequisito=1};

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarProductoRequisito(10, listaCambios, ProductoRequisitoActualizada);
            Assert.True(error.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", error.Errors[0].Description);
        }



        [Fact]
        public async void CrearProductoRequisitos_Ok()
        {
            //Arrange
            ProductoRequisito ProductoRequisito = new()
            {
                Id = 5,
                IdRequisito=1,
                IdTipoProducto=1
            };


            //Act
            ProductoRequisitoRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearProductoRequisito(ProductoRequisito);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProductoRequisito_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarProductoRequisito(999);
            Assert.True(error.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarProductoRequisito_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarProductoRequisito(1);

            Assert.False(error.IsError);
        }
        [Fact]
        public async Task ValidarProductoRequisitos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var result = await repository.ValidarProductoRequisitos(0,1, 1);

            //Assert
            Assert.False(result.IsError);
            Assert.True(result.Value);
        }
      
      

    

    }
}
