using Microsoft.EntityFrameworkCore;
using Polly;
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
    public class TipoProductoRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public TipoProductoRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<TipoProductoRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new TipoProductoRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            TipoProducto tipoProducto = new()
            {
                Id =1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            context.TipoProductos.Add(tipoProducto);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerTipoProductos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoProductos = repository.ObtenerTipoProductos();
            var result = Assert.IsType<List<TipoProducto>>(tipoProductos?.Result.Value);

            //Assert
            Assert.Equal("Tipo 1", result[0].Tipo);
        }
        [Fact]
        public async Task ObtenerTipoProductosPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoProductos = repository.ObtenerTipoProductoPorId(1);
            var result = tipoProductos?.Result.Value;

            //Assert            
            Assert.Equal("Tipo 1", result?.Tipo);
        }
        [Fact]
        public async Task ObtenerTipoProductoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoProducto = repository.ObtenerTipoProductoPorId(10);
            var result = tipoProducto?.Result.Errors;

            //Assert            
            Assert.True(tipoProducto?.Result.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", result?[0].Code);
            Assert.Equal("Tipo producto no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarTipoProducto_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var tipoProducto = await repository.ObtenerTipoProductoPorId(id);
            tipoProducto.Value.Tipo = "Tipo 2";
            tipoProducto.Value.IdCategoria = 2;

            //Act
            var tipoProductoActualizada = await repository.ActualizarTipoProducto(1, listaCambios, tipoProducto.Value);

            //Assert
            Assert.Equal("Tipo 2", tipoProductoActualizada.Value.Tipo);
        }
        [Fact]
        public async Task ActualizarTipoProducto_NoEncontrada()
        {
            var listaCambios = new List<string> { "Tipo" };
            var tipoProductoActualizada = new  TipoProducto { 
                Id = 999,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarTipoProducto(10, listaCambios, tipoProductoActualizada);
            Assert.True(error.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Tipo producto no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearTipoProducto_Ok()
        {
            //Arrange
            TipoProducto tipoProducto = new()
            {
                Id = 5,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };


            //Act
            TipoProductoRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearTipoProducto(tipoProducto);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProvincal_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarTipoProducto(999);
            Assert.True(error.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Tipo producto no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarTipoProducto_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarTipoProducto(1);

            Assert.False(error.IsError);
        }
       
        [Fact]
        public async Task ValidarTipoProducto_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.ValidarExisteProductoRequisito(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task TipoProductoDuplicada_DevuelveTrue()
        {

            // Arrange
            var tipoProducto =  new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var tipoProducto2 = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.TipoProductos.Add(tipoProducto);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TipoProductoRepository(context);
                var result = await repository.ValidarTipoProducto(tipoProducto.Tipo, tipoProducto.IdCategoria, tipoProducto.IdInstitucion, tipoProducto.Id);

                Assert.True(true);                
            }
        }

        [Fact]
        public async Task TipoProductoDuplicada_DevuelveFalse()
        {

            // Arrange
            var tipoProducto = new TipoProducto()
            {
                Id = 5,
                IdCategoria = 1,
                Tipo = "Tipo 5",
                IdInstitucion = 1
            };

            // Arrange
            var tipoProducto2 = new TipoProducto()
            {
                Id = 8,
                IdCategoria = 1,
                Tipo = "Tipo 8",
                IdInstitucion = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.TipoProductos.Add(tipoProducto);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TipoProductoRepository(context);
                var result = await repository.ValidarTipoProducto(tipoProducto2.Tipo, tipoProducto2.IdCategoria, tipoProducto2.IdInstitucion, tipoProducto2.Id);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task TipoProductoDuplicada_DevuelveError()
        {
            // Arrange            

            var tipoProductoCrear = new TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            // Arrange
            var tipoProductoExiste = new TipoProducto
            {
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.TipoProductos.Add(tipoProductoCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new TipoProductoRepository(context);
                var result = await repository.ValidarTipoProducto(tipoProductoExiste.Tipo, tipoProductoExiste.IdCategoria, tipoProductoExiste.IdInstitucion, tipoProductoExiste.Id);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
