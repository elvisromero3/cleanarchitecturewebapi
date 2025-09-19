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
    public class CasaRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public CasaRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<CasaRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new CasaRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Casa casa = new()
            {
                Id =1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            context.Casas.Add(casa);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerCasas_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var casas = repository.ObtenerCasas();
            var result = Assert.IsType<List<Casa>>(casas?.Result.Value);

            //Assert
            Assert.Equal("Casa 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerCasaPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var casas = repository.ObtenerCasaPorId(1);
            var result = casas?.Result.Value;

            //Assert            
            Assert.Equal("Casa 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerCasaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var casa = repository.ObtenerCasaPorId(10);
            var result = casa?.Result.Errors;

            //Assert            
            Assert.True(casa?.Result.IsError);
            Assert.Equal("Casa.NoEncontrada", result?[0].Code);
            Assert.Equal("Casa no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarCasa_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var casa = await repository.ObtenerCasaPorId(id);
            casa.Value.Nombre = "Casa 2";

            //Act
            var casaActualizada = await repository.ActualizarCasa(1, listaCambios, casa.Value);

            //Assert
            Assert.Equal("Casa 2", casaActualizada.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarCasa_NoEncontrada()
        {
            var listaCambios = new List<string> { "Nombre" };
            var casaActualizada = new  Casa { Id = 999, Nombre = "Nombre Casa" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCasa(10, listaCambios, casaActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Casa.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Casa no encontrada", error.Errors[0].Description);
        }



        [Fact]
        public async void CrearCasas_Ok()
        {
            //Arrange
            Casa casa = new()
            {
                Id = 5,
                Codigo = "1",
                Nombre = "Casa 1"
            };


            //Act
            CasaRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearCasa(casa);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarCasa_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCasa(999);
            Assert.True(error.IsError);
            Assert.Equal("Casa.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Casa no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCasa_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCasa(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task CasaDuplicada_DevuelveTrue()
        {

            // Arrange
            var casa = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var casa2 = new Casa()
            {
                Id = 3,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Casas.Add(casa);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CasaRepository(context);
                var result = await repository.ValidarCasa(casa2.Id, casa2.Codigo, casa2.Nombre);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task CasaDuplicada_DevuelveFalse()
        {

            // Arrange
            var casa = new Casa()
            {
                Id = 5,
                Codigo = "1",
                Nombre = "Casa 2"
            };

            // Arrange
            var casa2 = new Casa()
            {
                Id = 8,
                Codigo = "2",
                Nombre = "Casa 3"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Casas.Add(casa);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CasaRepository(context);
                var result = await repository.ValidarCasa(casa2.Id, casa2.Codigo, casa2.Nombre);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task CasaDuplicada_DevuelveError()
        {
            // Arrange            

            var casaCrear = new Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            // Arrange
            var casaExiste = new Casa
            {                
                Codigo = "1",
                Nombre = "Casa 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Casas.Add(casaCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CasaRepository(context);
                var result = await repository.ValidarCasa(casaExiste.Id, casaExiste.Codigo, casaExiste.Nombre);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
