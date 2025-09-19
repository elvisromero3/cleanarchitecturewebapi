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
    public class ProvinciaRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public ProvinciaRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<ProvinciaRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new ProvinciaRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Provincia provincia = new()
            {
                Id =1,                
                Nombre = "Provincia 1",
                Codigo = "01"
            };

            context.Provincias.Add(provincia);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerProvincias_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var provincias = repository.ObtenerProvincias();
            var result = Assert.IsType<List<Provincia>>(provincias?.Result.Value);

            //Assert
            Assert.Equal("Provincia 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerProvinciasPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var provincias = repository.ObtenerProvinciaPorId(1);
            var result = provincias?.Result.Value;

            //Assert            
            Assert.Equal("Provincia 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerProvinciaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var provincia = repository.ObtenerProvinciaPorId(10);
            var result = provincia?.Result.Errors;

            //Assert            
            Assert.True(provincia?.Result.IsError);
            Assert.Equal("Provincia.NoEncontrada", result?[0].Code);
            Assert.Equal("Provincia no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarProvincia_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var provincia = await repository.ObtenerProvinciaPorId(id);
            provincia.Value.Nombre = "Provincia 2";
            provincia.Value.Codigo = "02";

            //Act
            var provinciaActualizada = await repository.ActualizarProvincia(1, listaCambios, provincia.Value);

            //Assert
            Assert.Equal("Provincia 2", provinciaActualizada.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarProvincia_NoEncontrada()
        {
            var listaCambios = new List<string> { "Codigo" };
            var provinciaActualizada = new  Provincia { 
                Id = 999, 
                Nombre = "Nombre Provincia",
                Codigo = "01"
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarProvincia(10, listaCambios, provinciaActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Provincia.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Provincia no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async void CrearProvincia_Ok()
        {
            //Arrange
            Provincia provincia = new()
            {
                Id = 5,           
                Nombre = "Provincia 1",
                Codigo = "01"
            };


            //Act
            ProvinciaRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearProvincia(provincia);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarProvincal_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarProvincia(999);
            Assert.True(error.IsError);
            Assert.Equal("Provincia.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Provincia no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarProvincia_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarProvincia(1);

            Assert.False(error.IsError);
        }
        [Fact]
        public async Task ExisteRelacionConCanton_Ok()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.ExisteRelacionConCanton(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ProvinciaDuplicada_DevuelveTrue()
        {

            // Arrange
            var provincia = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "01"
            };

            var provincia2 = new Provincia()
            {
                Id = 3,
               Nombre = "Provincia 1",
                Codigo = "01"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Provincias.Add(provincia);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProvinciaRepository(context);
                var result = await repository.ValidarProvincia(provincia2.Id, provincia2.Codigo);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task ProvinciaDuplicada_DevuelveFalse()
        {

            // Arrange
            var provincia = new Provincia()
            {
                Id = 5,
                Nombre = "Provincia 2",
                Codigo = "01"
            };

            // Arrange
            var provincia2 = new Provincia()
            {
                Id = 8,
                Nombre = "Provincia 3",
                Codigo = "02"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Provincias.Add(provincia);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProvinciaRepository(context);
                var result = await repository.ValidarProvincia(provincia2.Id, provincia2.Codigo);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task ProvinciaDuplicada_DevuelveError()
        {
            // Arrange            

            var provinciaCrear = new Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "01"
            };

            // Arrange
            var provinciaExiste = new Provincia
            {                
                Nombre = "Provincia 1",
                Codigo = "01"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Provincias.Add(provinciaCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new ProvinciaRepository(context);
                var result = await repository.ValidarProvincia(provinciaExiste.Id,  provinciaExiste.Codigo);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
