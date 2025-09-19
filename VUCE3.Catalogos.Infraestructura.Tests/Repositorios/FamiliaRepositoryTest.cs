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
    public class FamiliaRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public FamiliaRepositoryTest()
        {
            var dbName = $"BDTestDFamilias_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<FamiliaRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new FamiliaRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Familia Familia = new()
            {
                Id =1,
                Nombre = "Familia 1"
            };

            context.Familias.Add(Familia);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerFamilias_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Familias = repository.ObtenerFamilias();
            var result = Assert.IsType<List<Familia>>(Familias?.Result.Value);

            //Assert
            Assert.Equal("Familia 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerFamiliaPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Familias = repository.ObtenerFamiliaPorId(1);
            var result = Familias?.Result.Value;

            //Assert            
            Assert.Equal("Familia 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerFamiliaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var Familia = repository.ObtenerFamiliaPorId(10);
            var result = Familia?.Result.Errors;

            //Assert            
            Assert.True(Familia?.Result.IsError);
            Assert.Equal("Familia.NoEncontrada", result?[0].Code);
            Assert.Equal("Familia no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarFamilia_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var Familia = await repository.ObtenerFamiliaPorId(id);
            Familia.Value.Nombre = "Familia 2";

            //Act
            var FamiliaActualizada = await repository.ActualizarFamilia(1, listaCambios, Familia.Value);

            //Assert
            Assert.Equal("Familia 2", FamiliaActualizada.Value.Nombre);
        }
        [Fact]
        public async Task ActualizarFamilia_NoEncontrada()
        {
            var listaCambios = new List<string> { "Nombre" };
            var FamiliaActualizada = new  Familia { Id = 999, Nombre = "Nombre Familia" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarFamilia(10, listaCambios, FamiliaActualizada);
            Assert.True(error.IsError);
            Assert.Equal("Familia.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Familia no encontrada", error.Errors[0].Description);
        }



        [Fact]
        public async void CrearFamilias_Ok()
        {
            //Arrange
            Familia Familia = new()
            {
                Id = 5,
                Nombre = "Familia 1"
            };


            //Act
            FamiliaRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearFamilia(Familia);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarFamilia_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarFamilia(999);
            Assert.True(error.IsError);
            Assert.Equal("Familia.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Familia no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarFamilia_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarFamilia(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task FamiliaDuplicada_DevuelveTrue()
        {

            // Arrange
            var Familia = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var Familia2 = new Familia()
            {
                Id = 3,
                Nombre = "Familia 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(Familia);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new FamiliaRepository(context);
                var result = await repository.ValidarFamilia(Familia2.Id, Familia2.Nombre);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task FamiliaDuplicada_DevuelveFalse()
        {

            // Arrange
            var Familia = new Familia()
            {
                Id = 5,
                Nombre = "Familia 2"
            };

            // Arrange
            var Familia2 = new Familia()
            {
                Id = 8,
                Nombre = "Familia 3"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(Familia);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new FamiliaRepository(context);
                var result = await repository.ValidarFamilia(Familia2.Id, Familia2.Nombre);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task FamiliaDuplicada_DevuelveError()
        {
            // Arrange            

            var FamiliaCrear = new Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            // Arrange
            var FamiliaExiste = new Familia
            {
                Nombre = "Familia 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Familias.Add(FamiliaCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new FamiliaRepository(context);
                var result = await repository.ValidarFamilia(FamiliaExiste.Id, FamiliaExiste.Nombre);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
