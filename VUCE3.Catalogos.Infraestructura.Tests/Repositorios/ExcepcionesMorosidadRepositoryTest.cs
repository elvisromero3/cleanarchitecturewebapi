using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using Moq;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class ExcepcionesMorosidadRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public ExcepcionesMorosidadRepositoryTest()
        {
            var dbName = $"BDTestExcepcionesMorosidad_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<ExcepcionesMorosidadRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new ExcepcionesMorosidadRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa ='J',
                NumeroIdentificacionEmpresa ="123DF",
                NombreEmpresa= "Export China",
                Observaciones="",
                IdTipoTramite = 2,
                IdSubtipoTramite = 2,
                IdRegimen = 2,
                IdTipoAccion = 2,
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(10)

            };
            context.ExcepcionesMorosidad.Add(excepcion);

            var excepcion2 = new ExcepcionMorosidad()
            {
                Id = 2,
                TipoIdentificacionEmpresa = 'J',
                NumeroIdentificacionEmpresa = "123DF",
                NombreEmpresa = "Export China",
                Observaciones = "Texto de obsevaciones",
                IdTipoTramite = 1,
                IdSubtipoTramite = 1,
                IdRegimen = 1,
                IdTipoAccion = 1,
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(10)
            };

            context.ExcepcionesMorosidad.Add(excepcion2);

            var empresa = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1,
            };
            context.Empresas.Add(empresa);

            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerExcepcionMorosidad_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var excepciones = repository.ObtenerExcepcionesMorosidad();
            var result = Assert.IsType<List<ExcepcionMorosidad>>(excepciones?.Result.Value);

            //Assert
           // Assert.Equal(1, result[0].Id);
        }
        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var excepcion = repository.ObtenerExcepcionMorosidadPorId(1);
            var result = excepcion?.Result.Value;

            //Assert            
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_NoEncontrada()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var excepcion = repository.ObtenerExcepcionMorosidadPorId(10);
            var result = excepcion?.Result.Errors;

            //Assert            
            Assert.True(excepcion?.Result.IsError);
            Assert.Equal("ExcepcionMorosidad.NoEncontrada", result?[0].Code);
            Assert.Equal("Excepción morosidad no encontrada", result?[0].Description);
        }

        [Fact]
        public async void CrearExcepcionMorosidad_Ok()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 3,
            };

            //Act
            ExcepcionesMorosidadRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearExcepcionMorosidad(excepcion);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Observaciones" };
            var repository = await CreateRepositoryAsync();

            var excepcion = await repository.ObtenerExcepcionMorosidadPorId(id);
            excepcion.Value.Observaciones = "test";

            //Act
            var excepcionActualizada = await repository.ActualizarExcepcionMorosidad(1, listaCambios, excepcion.Value);

            //Assert
            Assert.Equal("test", excepcionActualizada.Value.Observaciones);
        }
        [Fact]
        public async Task ActualizarExcepcionMorosidad_NoEncontrada()
        {
            var listaCambios = new List<string> { "Observaciones" };
            var excepcionActualizada = new ExcepcionMorosidad { Id = 999 };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarExcepcionMorosidad(10, listaCambios, excepcionActualizada);
            Assert.True(error.IsError);
            Assert.Equal("ExcepcionMorosidad.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Excepción morosidad no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarExcepcionMorosidad_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarExcepcionMorosidad(999);
            Assert.True(error.IsError);
            Assert.Equal("ExcepcionMorosidad.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Excepción morosidad no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarExcepcionMorosidad_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarExcepcionMorosidad(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarExcepcionMorosidad_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 0,
                TipoIdentificacionEmpresa = 'J',
                NumeroIdentificacionEmpresa = "123DF",
                NombreEmpresa = "Export China",
                Observaciones = "Texto de obsevaciones",
                IdTipoTramite = 1,
                IdSubtipoTramite = 1,
                IdRegimen = 1,
                IdTipoAccion = 1,
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(10)
            };

            //Act
            var result = await repository.ValidarExcepcionMorosidad(excepcion, It.IsAny<bool>());
            //Assert
            Assert.False(result.IsError);
        }

    }
}
