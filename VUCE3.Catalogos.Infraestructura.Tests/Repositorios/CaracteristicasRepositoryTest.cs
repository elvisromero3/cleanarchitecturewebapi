using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Migrations;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CaracteristicasRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public CaracteristicasRepositoryTest()
        {
            var dbName = $"BDTestDCaracteristicas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<CaracteristicasRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new CaracteristicasRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Caracteristica caracteristica = new()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            context.Caracteristicas.Add(caracteristica);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerCaracteristicas_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var caracteristica = repository.ObtenerCaracteristicas();
            var result = Assert.IsType<List<Caracteristica>>(caracteristica?.Result.Value);

            //Assert
            Assert.Equal("Caracteristica 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerCaracteristicaPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var caracteristicas = repository.ObtenerCaracteristicaPorId(1);
            var result = caracteristicas?.Result.Value;

            //Assert            
            Assert.Equal("Caracteristica 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerCaracteristicaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var caracteristica = repository.ObtenerCaracteristicaPorId(10);
            var result = caracteristica?.Result.Errors;

            //Assert            
            Assert.True(caracteristica?.Result.IsError);
            Assert.Equal("Caracteristica.NoEncontrada", result?[0].Code);
            Assert.Equal("Característica no encontrada", result?[0].Description);
        }

        [Fact]
        public async void CrearCaracteristica_Ok()
        {
            //Arrange
            Caracteristica caracteristica = new()
            {
                Id = 5,
                IdInstitucion = 5,
                Nombre = "Caracteristica 5"
            };

            //Act
            CaracteristicasRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearCaracteristica(caracteristica);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCaracteristica_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var caracteristica = await repository.ObtenerCaracteristicaPorId(id);
            caracteristica.Value.Nombre = "Caracteristica 2";

            //Act
            var caracteristicaActualizado = await repository.ActualizarCaracteristica(caracteristica.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Caracteristica 2", caracteristicaActualizado.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarCaracteristica_NoEncontrado()
        {
            var listaCambios = new List<string> { "Nombre" };
            var caracteristicaActualizada = new Caracteristica { Id = 999, Nombre = "Nombre Caracteristica" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCaracteristica(caracteristicaActualizada, 10, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Caracteristica.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Característica no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCaracteristica_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCaracteristica(999);
            Assert.True(error.IsError);
            Assert.Equal("Caracteristica.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Característica no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCaracteristica_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCaracteristica(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarCaracteristica_False()
        {
            var repository = await CreateRepositoryAsync();
            var caracteristica = new Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };

            var result = await repository.ValidarCaracteristica(caracteristica.Id, caracteristica.IdInstitucion, caracteristica.Nombre);

            Assert.False(result.IsError);
            Assert.False(result.Value);
        }
    }
}
