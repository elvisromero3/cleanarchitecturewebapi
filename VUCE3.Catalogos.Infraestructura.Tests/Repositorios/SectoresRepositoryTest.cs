using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Persistencia;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class SectoresRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public SectoresRepositoryTest()
        {
            var dbName = $"BDTestDSectores_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<SectoresRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new
            CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new SectoresRepository(context);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            Sector sector = new()
            {
                Id = 1,               
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            context.Sectores.Add(sector);
            await context.SaveChangesAsync();

        }

        [Fact]
        public async Task ObtenerSectores_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var sectores = repository.ObtenerSectores();
            var result = Assert.IsType<List<Sector>>(sectores?.Result.Value);

            //Assert
            Assert.Equal("Sector 1", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerSectorPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var sectores = repository.ObtenerSectorPorId(1);
            var result = sectores?.Result.Value;

            //Assert            
            Assert.Equal("Sector 1", result?.Nombre);
        }
        [Fact]
        public async Task ObtenerSectorPorId_Error()
        {
            // Arrange  
            var repository = await CreateRepositoryAsync();

            // Act  
            var sector = await repository.ObtenerSectorPorId(10);
            var result = sector.FirstError;

            // Assert  
            Assert.True(sector.IsError);
            Assert.Equal("Sector.NoEncontrado", result.Code);
            Assert.Equal("Sector no encontrado", result.Description);
        }

        [Fact]
        public async void CrearSector_Ok()
        {
            //Arrange
            Sector sector = new()
            {
                Id = 5,               
                Nombre = "Sector 5",
                Codigo = "Codigo 5"
            };

            //Act
            SectoresRepository repository = await CreateRepositoryAsync();

            var result = await repository.CrearSector(sector);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarSector_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre" };
            var repository = await CreateRepositoryAsync();

            var sector = await repository.ObtenerSectorPorId(id);
            sector.Value.Nombre = "Sector 2";

            //Act
            var sectorActualizado = await repository.ActualizarSector(sector.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Sector 2", sectorActualizado.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarSector_NoEncontrado()
        {
            var listaCambios = new List<string> { "Nombre" };
            var sectorActualizado = new Sector { Id = 999, Nombre = "Nombre Sector" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarSector(sectorActualizado,10, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Sector.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Sector no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarSector_NoEncontrado()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarSector(999);
            Assert.True(error.IsError);
            Assert.Equal("Sector.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Sector no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarSector_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarSector(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ValidarSector_False()
        {
            var repository = await CreateRepositoryAsync();
            var sector = new Sector { Id = 1, Nombre = "Sector 1", Codigo = "Codigo 1" };

            var result = await repository.ValidarSector(sector.Id,sector.Nombre, sector.Codigo);

            Assert.False(result.IsError);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ObtenerSectorPorNombre_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            //Act
            var sectores = repository.ObtenerSectorPorNombre("Sector 1");
            var result = sectores?.Result.Value;
            //Assert            
            Assert.Equal("Sector 1", result?.Nombre);
        }

        [Fact]
        public async Task ObtenerSectorPorNombre_Error()
        {
            // Arrange  
            var repository = await CreateRepositoryAsync();

            // Act  
            var sector = await repository.ObtenerSectorPorNombre("Sector 10");
            var result = sector.Errors;

            // Assert  
            Assert.True(sector.IsError);
            Assert.Equal("Sector.NoEncontrado", result?[0].Code);
            Assert.Equal("Sector no encontrado", result?[0].Description);
        }
    }
}
