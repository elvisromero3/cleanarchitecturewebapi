using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class BloqueComercialRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public BloqueComercialRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<BloqueComercialRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new BloqueComercialRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            BloqueComercial bloqueComercial = new()
            {
                Id =1,                
                Nombre = "BloqueComercial 1",
              
            };

            context.BloquesComerciales.Add(bloqueComercial);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerBloquesComerciales_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var BloquesComerciales = repository.ObtenerBloquesComerciales();
            var result = Assert.IsType<List<BloqueComercial>>(BloquesComerciales?.Result.Value);

            //Assert
            Assert.Equal("BloqueComercial 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerBloquesComercialesPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var BloquesComerciales = repository.ObtenerBloqueComercialPorId(1);
            var result = BloquesComerciales?.Result.Value;

            //Assert            
            Assert.Equal("BloqueComercial 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerBloqueComercialPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var bloqueComercial = repository.ObtenerBloqueComercialPorId(10);
            var result = bloqueComercial?.Result.Errors;

            //Assert            
            Assert.True(bloqueComercial?.Result.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", result?[0].Code);
            Assert.Equal("BloqueComercial no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarBloqueComercial_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var bloqueComercial = await repository.ObtenerBloqueComercialPorId(id);
            bloqueComercial.Value.Nombre = "BloqueComercial 2";
        

            //Act
            var bloqueComercialActualizada = await repository.ActualizarBloqueComercial(1, listaCambios, bloqueComercial.Value);

            //Assert
            Assert.Equal("BloqueComercial 2", bloqueComercialActualizada.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarBloqueComercial_NoEncontrada()
        {
            var listaCambios = new List<string> { "Codigo" };
            var bloqueComercialActualizada = new  BloqueComercial { 
                Id = 999, 
                Nombre = "Nombre BloqueComercial",
              
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarBloqueComercial(10, listaCambios, bloqueComercialActualizada);
            Assert.True(error.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("BloqueComercial no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearBloqueComercial_Ok()
        {
            //Arrange
            BloqueComercial bloqueComercial = new()
            {
                Id = 5,           
                Nombre = "BloqueComercial 1",
             
            };


            //Act
            BloqueComercialRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearBloqueComercial(bloqueComercial);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProvincal_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarBloqueComercial(999);
            Assert.True(error.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("BloqueComercial no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarBloqueComercial_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarBloqueComercial(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task BloqueComercialDuplicada_DevuelveTrue()
        {

            // Arrange
            var bloqueComercial = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
             
            };

            var bloqueComercial2 = new BloqueComercial()
            {
                Id = 3,
               Nombre = "BloqueComercial 1",
              
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.BloquesComerciales.Add(bloqueComercial);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new BloqueComercialRepository(context);
                var result = await repository.ValidarBloqueComercial(bloqueComercial2.Id, bloqueComercial2.Nombre);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task BloqueComercialDuplicada_DevuelveFalse()
        {

            // Arrange
            var bloqueComercial = new BloqueComercial()
            {
                Id = 5,
                Nombre = "BloqueComercial 2",
             
            };

            // Arrange
            var bloqueComercial2 = new BloqueComercial()
            {
                Id = 8,
                Nombre = "BloqueComercial 3",
             
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.BloquesComerciales.Add(bloqueComercial);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new BloqueComercialRepository(context);
                var result = await repository.ValidarBloqueComercial(bloqueComercial2.Id, bloqueComercial2.Nombre);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task BloqueComercialDuplicada_DevuelveError()
        {
            // Arrange            

            var bloqueComercialCrear = new BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
             
            };

            // Arrange
            var bloqueComercialExiste = new BloqueComercial
            {                
                Nombre = "BloqueComercial 1",
               
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.BloquesComerciales.Add(bloqueComercialCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new BloqueComercialRepository(context);
                var result = await repository.ValidarBloqueComercial(bloqueComercialExiste.Id,  bloqueComercialExiste.Nombre);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
