using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class TiposAccionesExcepcionesMorosidadRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public TiposAccionesExcepcionesMorosidadRepositoryTest()
        {
            var dbName = $"BDTestTiposAccionesExcepcionesMorosidad_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<TiposAccionesExcepcionesMorosidadRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new TiposAccionesExcepcionesMorosidadRepository(context);
        }

        [Fact]
        public async Task ObtenerTiposAccionesExcepcionesMorosidad_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoTramites = repository.ObtenerTiposAccionesExcepcionesMorosidad();
            var result = Assert.IsType<List<TipoAccionExcepcionMorosidad>>(tipoTramites?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Acción 1", result[0].Nombre);
        }

        [Fact]
        public async Task ObtenerTipoAccionExcepcionMorosidadPorId_DevuelveOk()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoTramite = repository.ObtenerTipoAccionExcepcionMorosidadPorId(1);
            var result = tipoTramite?.Result.Value;

            //Assert
            Assert.False(tipoTramite?.Result.IsError);
            Assert.Equal("Acción 1", result?.Nombre);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task ObtenerTipoAccionExcepcionMorosidadPorId_DevuelveNoEncontrado()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var tipoEmpresa = repository.ObtenerTipoAccionExcepcionMorosidadPorId(99);

            //Assert
            Assert.True(tipoEmpresa?.Result.IsError);
            Assert.Equal("TipoAccionExcepcionMorosidad.NoEncontrado", tipoEmpresa?.Result.Errors[0].Code);
            Assert.Equal("Tipo acción excepción morosidad no encontrado", tipoEmpresa?.Result.Errors[0].Description);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var tipo1 = new TipoAccionExcepcionMorosidad
            {
                Id = 1,
                Nombre = "Acción 1"
            };
            var tipo2 = new TipoAccionExcepcionMorosidad
            {
                Id = 2,
                Nombre = "Acción 2"
            };

            context.TiposAccionesExcepcionesMorosidad.Add(tipo1);
            context.TiposAccionesExcepcionesMorosidad.Add(tipo2);

            await context.SaveChangesAsync();
        }
    }
}
