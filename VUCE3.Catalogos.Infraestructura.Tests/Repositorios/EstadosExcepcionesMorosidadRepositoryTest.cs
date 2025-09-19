using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class EstadosExcepcionesMorosidadRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public EstadosExcepcionesMorosidadRepositoryTest()
        {
            var dbName = $"BDTestEstadosExcepcionesMorosidad_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<EstadosExcepcionesMorosidadRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new EstadosExcepcionesMorosidadRepository(context);
        }

        [Fact]
        public async Task ObtenerEstadosExcepcionesMorosidad_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var estados = repository.ObtenerEstadosExcepcionesMorosidad();
            var result = Assert.IsType<List<EstadoExcepcionMorosidad>>(estados?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Estado 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerEstadoExcepcionMorosidadPorId_DevuelveOk()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var estado = repository.ObtenerEstadoExcepcionMorosidadPorId(1);
            var result = estado?.Result.Value;

            //Assert
            Assert.False(estado?.Result.IsError);
            Assert.Equal("Estado 1", result?.Nombre);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task ObtenerTipoTramitePorId_DevuelveNoEncontrado()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoEmpresa = repository.ObtenerEstadoExcepcionMorosidadPorId(99);

            //Assert
            Assert.True(tipoEmpresa?.Result.IsError);
            Assert.Equal("EstadoExcepcionMorosidad.NoEncontrado", tipoEmpresa?.Result.Errors[0].Code);
            Assert.Equal("Estado excepción morosidad no encontrado", tipoEmpresa?.Result.Errors[0].Description);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var estado1 = new EstadoExcepcionMorosidad
            {
                Id = 1,
                Nombre = "Estado 1"
            };
            var estado2 = new EstadoExcepcionMorosidad
            {
                Id = 2,
                Nombre = "Estado 2"
            };

            context.EstadosExcepcionesMorosidad.Add(estado1);
            context.EstadosExcepcionesMorosidad.Add(estado2);

            await context.SaveChangesAsync();
        }
    }
}
