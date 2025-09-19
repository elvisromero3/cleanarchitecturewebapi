using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class ProductosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public ProductosRepositoryTest()
        {
            var dbName = $"BDTestDProductos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<ProductosRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new ProductosRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Productos Productos = new()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            context.Productos.Add(Productos);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerProductos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Productos = repository.ObtenerProductos();
            var result = Assert.IsType<List<Productos>>(Productos?.Result.Value);

            //Assert
            Assert.Equal("Nombre Comun 1", result[0].NombreComun);
        }
        [Fact]
        public async Task ObtenerProductosPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Productos = repository.ObtenerProductoPorId(1);
            var result = Productos?.Result.Value;

            //Assert            
            Assert.Equal("Nombre Comun 1", result?.NombreComun);
        }
        [Fact]
        public async Task ObtenerProductosPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Productos = repository.ObtenerProductoPorId(10);
            var result = Productos?.Result.Errors;

            //Assert            
            Assert.True(Productos?.Result.IsError);
            Assert.Equal("Productos.NoEncontrado", result?[0].Code);
            Assert.Equal("Producto no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarProductos_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "NombreComun" };
            var repository = await CreateRepositoryAsync();

            var Productos = await repository.ObtenerProductoPorId(id);
            Productos.Value.NombreComun = "Nombre Comun 1";

            //Act
            var ProductosActualizada = await repository.ActualizarProductos(1, listaCambios, Productos.Value);

            //Assert
            Assert.Equal("Nombre Comun 1", ProductosActualizada.Value.NombreComun);
        }
        [Fact]
        public async Task ActualizarProductos_NoEncontrada()
        {
            var listaCambios = new List<string> { "NombreComun" };
            var ProductosActualizada = new  Productos { Id = 999, NombreComun = "Nombre Comun 1" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarProductos(10, listaCambios, ProductosActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Productos.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Producto no encontrado", error.Errors[0].Description);
        }



        [Fact]
        public async void CrearProductos_Ok()
        {
            //Arrange
            Productos Productos = new()
            {
                Id = 10,
                Clase = "Clase 3",
                Presentacion = "Presentacion 3",
                NombreComun = "Nombre Comun 3",
                NombreCientifico = "Nombre Cientifico 3",
                Tradicional = true
            };


            //Act
            ProductosRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearProductos(Productos);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProductos_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarProductos(999);
            Assert.True(error.IsError);
            Assert.Equal("Productos.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Producto no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarProductos_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarProductos(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ProductosDuplicada_DevuelveTrue()
        {

            // Arrange
            var Productos = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var Productos2 = new Productos()
            {
                Id = 3,
                Clase = "Clase 3",
                Presentacion = "Presentacion 3",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Productos.Add(Productos);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProductosRepository(context);
                var result = await repository.ValidarProductos(Productos2.Id, Productos2.NombreComun, Productos2.NombreCientifico);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task ProductosDuplicada_DevuelveFalse()
        {

            // Arrange
            var Productos = new Productos()
            {
                Id = 2,
                Clase = "Clase 2",
                Presentacion = "Presentacion 2",
                NombreComun = "Nombre Comun 2",
                NombreCientifico = "Nombre Cientifico 2",
                Tradicional = true
            };

            // Arrange
            var Productos2 = new Productos()
            {
                Id = 8,
                Clase = "Clase 8",
                Presentacion = "Presentacion 8",
                NombreComun = "Nombre Comun 8",
                NombreCientifico = "Nombre Cientifico 8",
                Tradicional = true
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Productos.Add(Productos);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProductosRepository(context);
                var result = await repository.ValidarProductos(Productos2.Id, Productos2.NombreComun, Productos2.NombreCientifico);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task ProductosDuplicada_DevuelveError()
        {
            // Arrange            

            var ProductosCrear = new Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            // Arrange
            var ProductosExiste = new Productos
            {
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Productos.Add(ProductosCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProductosRepository(context);
                var result = await repository.ValidarProductos(ProductosExiste.Id, ProductosExiste.NombreComun, ProductosExiste.NombreCientifico);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
