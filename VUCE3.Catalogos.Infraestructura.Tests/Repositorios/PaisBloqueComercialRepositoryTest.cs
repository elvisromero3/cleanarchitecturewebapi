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
    public class PaisBloqueComercialRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public PaisBloqueComercialRepositoryTest()
        {
            var dbName = $"BDTestDPaisBloqueComercials_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<PaisBloqueComercialRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new PaisBloqueComercialRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            PaisBloqueComercial paisBloqueComercial = new()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            context.PaisBloqueComercial.Add(paisBloqueComercial);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerPaisBloqueComercials_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paisBloqueComercials = repository.ObtenerPaisBloqueComercial();
            var result = Assert.IsType<List<PaisBloqueComercial>>(paisBloqueComercials?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].IdBloqueComercial);
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paisBloqueComercials = repository.ObtenerPaisBloqueComercialPorId(1);
            var result = paisBloqueComercials?.Result.Value;

            //Assert            
            Assert.Equal(1, result?.IdBloqueComercial);
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var paisBloqueComercial = repository.ObtenerPaisBloqueComercialPorId(10);
            var result = paisBloqueComercial?.Result.Errors;

            //Assert            
            Assert.True(paisBloqueComercial?.Result.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", result?[0].Code);
            Assert.Equal("Pais de bloque comercial no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarPaisBloqueComercial_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "IdBloqueComercial" };
            var repository = await CreateRepositoryAsync();

            var paisBloqueComercial = await repository.ObtenerPaisBloqueComercialPorId(id);
            paisBloqueComercial.Value.IdBloqueComercial = 1;

            //Act
            var paisBloqueComercialActualizada = await repository.ActualizarPaisBloqueComercial(1, listaCambios, paisBloqueComercial.Value);

            //Assert
            Assert.Equal(1, paisBloqueComercialActualizada.Value.IdBloqueComercial);
        }
        [Fact]
        public async Task ActualizarPaisBloqueComercial_NoEncontrada()
        {
            var listaCambios = new List<string> { "IdBloqueComercial" };
            var paisBloqueComercialActualizada = new  PaisBloqueComercial { Id = 999, IdBloqueComercial = 1,IdPais = 1 };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarPaisBloqueComercial(10, listaCambios, paisBloqueComercialActualizada);
            Assert.True(error.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Pais de bloque comercial no encontrada", error.Errors[0].Description);
        }



        [Fact]
        public async void CrearPaisBloqueComercials_Ok()
        {
            //Arrange
            PaisBloqueComercial paisBloqueComercial = new()
            {
                Id = 2,
                IdBloqueComercial = 2,
                IdPais = 1
            };


            //Act
            PaisBloqueComercialRepository repository = await CreateRepositoryAsync();
         
            var result = await repository.CrearPaisBloqueComercial(paisBloqueComercial);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercial_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarPaisBloqueComercial(999);
            Assert.True(error.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Pais de bloque comercial no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercial_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarPaisBloqueComercial(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task PaisBloqueComercialDuplicada_DevuelveTrue()
        {

            // Arrange
            var paisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var paisBloqueComercial2 = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.PaisBloqueComercial.Add(paisBloqueComercial);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new PaisBloqueComercialRepository(context);
                var result = await repository.ValidarPaisBloqueComercial(paisBloqueComercial2.IdBloqueComercial, paisBloqueComercial2.IdPais);

                Assert.True(result.Value);                
            }
        }

        [Fact]
        public async Task PaisBloqueComercialDuplicada_DevuelveFalse()
        {

            // Arrange
            var paisBloqueComercial = new PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            // Arrange
            var paisBloqueComercial2 = new PaisBloqueComercial()
            {
                Id = 2,
                IdBloqueComercial = 2,
                IdPais = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.PaisBloqueComercial.Add(paisBloqueComercial);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new PaisBloqueComercialRepository(context);
                var result = await repository.ValidarPaisBloqueComercial(paisBloqueComercial2.IdBloqueComercial, paisBloqueComercial2.IdPais);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task PaisBloqueComercialDuplicada_DevuelveError()
        {
            // Arrange            

            var paisBloqueComercialCrear = new PaisBloqueComercial()
            {
                Id = 2,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            // Arrange
            var paisBloqueComercialExiste = new PaisBloqueComercial
            {
                Id = 2,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.PaisBloqueComercial.Add(paisBloqueComercialCrear);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new PaisBloqueComercialRepository(context);
                var result = await repository.ValidarPaisBloqueComercial(paisBloqueComercialExiste.IdBloqueComercial, paisBloqueComercialExiste.IdPais);
                Assert.True(result.Value);
                Assert.Equal("ErrorOr.NoErrors", result.Errors[0].Code);
            }
        }

    }
}
