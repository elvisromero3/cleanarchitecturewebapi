using ErrorOr;
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
    public class EstablecimientosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public EstablecimientosRepositoryTest()
        {
            var dbName = $"BDTestDCasas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }
        private async Task<EstablecimientosRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new EstablecimientosRepository(context);
        }
        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            List<Establecimiento> establecimientos = new List<Establecimiento>()
            {
                new Establecimiento
                {
                    Id = 1,
                    NumeroCvo = "123456",
                    NombreEstablecimiento = "Nombre establecimiento 1",
                    ActividadPrimaria = "Actividad1",
                    ActividadSecundaria = "Actividad2",
                    Provincia = "Provincia1",
                    Canton = "Canton1",
                    Distrito = "Distrito1",
                    DireccionExacta = "351 Calle Las Rosas",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                     EstadoEstablecimiento = "Activo",
                },
                new Establecimiento
                {
                    Id = 2,
                    NumeroCvo = "654321",
                    NombreEstablecimiento = "Nombre establecimiento 2",
                    ActividadPrimaria = "Actividad1",
                    ActividadSecundaria = "Actividad2",
                    Provincia = "Provincia1",
                    Canton = "Canton1",
                    Distrito = "Distrito1",
                    DireccionExacta = "351 Calle Las Rosas",
                    FechaVencimiento = DateTime.Now.AddDays(3),
                     EstadoEstablecimiento = "Activo",
                },
                new Establecimiento
                {
                    Id = 3,
                    NumeroCvo = "654123",
                    NombreEstablecimiento = "Nombre establecimiento 3",
                    ActividadPrimaria = "Actividad1",
                    ActividadSecundaria = "Actividad2",
                    Provincia = "Provincia1",
                    Canton = "Canton1",
                    Distrito = "Distrito1",
                    DireccionExacta = "351 Calle Las Rosas",
                    FechaVencimiento = DateTime.Now.AddDays(7),
                     EstadoEstablecimiento = "Activo",
                },
            };

            context.Establecimientos.AddRange(establecimientos);
            await context.SaveChangesAsync();
        }

        // Pruebas de lectura de Establecimientos

        [Fact]
        public async Task ObtenerEstablecimientos_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            
            //Act
            var establecimientos = repository.ObtenerEstablecimientos();
            var result = Assert.IsType<List<Establecimiento>>(establecimientos?.Result.Value);
            
            //Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task ObtenerEstablecimientoPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var establecimiento = repository.ObtenerEstablecimientoPorId(1);
            var result = establecimiento?.Result.Value;

            //Assert            
            Assert.Equal("123456", result?.NumeroCvo);
        }

        [Fact]
        public async Task ObtenerEstablecimientoPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            //Act
            var establecimiento = repository.ObtenerEstablecimientoPorId(4);
            var result = establecimiento?.Result.FirstError;
            //Assert            
            Assert.True(establecimiento?.Result.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, result);
        }

        // Pruebas de creacion de establecimientos

        [Fact]
        public async Task CrearEstablecimiento_Ok()
        {
            //Arrange
            var establecimiento = new Establecimiento()
            {
                NumeroCvo = "745896",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var result = await repository.CrearEstablecimiento(establecimiento);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("745896", result.Value.NumeroCvo);
        }

        // Pruebas de actualizacion de establecimientos

        [Fact]
        public async Task ActualizarEstablecimiento_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            Establecimiento establecimiento = new Establecimiento
            {
                Id = 1,
                NumeroCvo = "12345600",
            };
            //Act
            var establecimientoActualizado = repository.ActualizarEstablecimiento(establecimiento.Id, new List<string> { "NumeroCvo" }, establecimiento);
            var result = establecimientoActualizado?.Result.Value;
            //Assert            
            Assert.Equal("12345600", result?.NumeroCvo);
        }

        [Fact]
        public async Task ActualizarEstablecimiento_ErrorNoEncontrado()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            Establecimiento establecimiento = new Establecimiento
            {
                Id = 999,
                NumeroCvo = "12345600",
            };
            //Act
            var establecimientoActualizado = repository.ActualizarEstablecimiento(establecimiento.Id, new List<string> { "NumeroCvo" }, establecimiento);
            var result = establecimientoActualizado?.Result.FirstError;
            //Assert            
            Assert.True(establecimientoActualizado?.Result.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, result);
        }

        // Pruebas de eliminacion de establecimientos

        [Fact]
        public async Task EliminarEstablecimiento_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var establecimiento = await repository.EliminarEstablecimiento(1);

            //Assert
            Assert.False(establecimiento.IsError);
            Assert.Equal(Result.Deleted, establecimiento.Value);
        }

        [Fact]
        public async Task EliminarEstablecimiento_ErrorNoEncontrado()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            
            //Act
            var establecimiento = await repository.EliminarEstablecimiento(999);
            
            //Assert
            Assert.True(establecimiento.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, establecimiento.FirstError);
        }

        [Fact]
        public async Task ValidarEstablecimientoe_Ok()
        {
            //Arrange

            var repository = await CreateRepositoryAsync();

            //Act
            var result = await repository.ValidarEstablecimiento(2, "654321");

            //Assert
            Assert.False(result.Value);
        }
    }
}
