using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class ProfesionalesTestRepository
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public ProfesionalesTestRepository()
        {
            var dbName = $"BDTestProfesionales_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;        
        }        

        private async Task<ProfesionalesRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new ProfesionalesRepository(context);
        }

        [Fact]
        public async Task ObtenerProfesionales_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var profesionales = repository.ObtenerProfesionales();
            var result = Assert.IsType<List<Profesional>>(profesionales?.Result.Value);

            //Assert
            Assert.Equal("Pedro Perez", result[0].Nombre);            
        }
        [Fact]
        public async Task ObtenerProfesionalesIdInstitucion_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var profesionales = repository.ObtenerProfesionalesPorIdInstitucion(1);
            var result = Assert.IsType<List<Profesional>>(profesionales?.Result.Value);

            //Assert
            Assert.Equal("Pedro Perez", result[0].Nombre);
        }
        [Fact]
        public async Task ObtenerProfesionalPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var profesional = repository.ObtenerProfesionalPorId(1);
            var result = profesional?.Result.Value;

            //Assert            
            Assert.Equal("Pedro Perez", result?.Nombre);            
        }

        [Fact]
        public async Task ObtenerProfesionalPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var profesional = repository.ObtenerProfesionalPorId(10);
            var result = profesional?.Result.Errors;

            //Assert            
            Assert.True(profesional?.Result.IsError);
            Assert.Equal("Regente.NoEncontrado", result?[0].Code);
            Assert.Equal("Regente no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarProfesional_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre profesional cambio" };
            var repository = await CreateRepositoryAsync();

            var profesional = await repository.ObtenerProfesionalPorId(id);
            profesional.Value.Nombre = "Nombre profesional";

            //Act
            var profesionalActualizado = await repository.ActualizarProfesional(profesional.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Nombre profesional", profesionalActualizado.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarProfesional_NoEncontrada()
        {
            var listaCambios = new List<string> { "Pinto" };
            var profesionalActualizado = new Profesional { Id = 999, Nombre = "Nombre profesional" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarProfesional(profesionalActualizado, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Regente.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Regente no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task CrearProfesional_Ok()
        {
            var profesional = new Profesional()
            {
                Id = 14,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var profesionalNuevo = await repository.CrearProfesional(profesional);

            //Assert
            var result = Assert.IsType<ErrorOr<Profesional>>(profesionalNuevo);
            Assert.Equal(14, result.Value.Id);
            Assert.Equal("Pedro Perez", result.Value.Nombre);            
        }

        [Fact]
        public async Task EliminarProfesional_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarProfesional(999);
            Assert.True(error.IsError);
            Assert.Equal("Regente.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Regente no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarProfesional_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarProfesional(1);

            Assert.False(error.IsError);
        }
        
        [Fact]
        public async Task ProfesionalDuplicado_DevuelveTrue()
        {

            // Arrange
            var profesional = new Profesional()
            {
                Id = 2,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion= 1
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ProfesionalDuplicado_DevuelveFalse()
        {

            // Arrange
            var profesional = new Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ProfesionalDuplicado_MismoDevuelveFalse()
        {

            // Arrange
            var profesional = new Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion);
            Assert.False(result.Value);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {

            var profesional1 = new Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion =1
            };

            var profesional2 = new Profesional()
            {
                Id = 2,
                Nombre = "Pedro Sanchez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123457",
                Profesion = "Comerciante",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion =1
            };

            var profesional3 = new Profesional()
            {
                Id = 3,
                Nombre = "Pedro Estrada",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123458",
                Profesion = "Maestro",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion =1

            };

            context.Profesionales.Add(profesional1);
            context.Profesionales.Add(profesional2);
            context.Profesionales.Add(profesional3);            

            await context.SaveChangesAsync();
        }

    }
}
