using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class EmpresasTestRepository
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public EmpresasTestRepository()
        {
            var dbName = $"BDTestEmpresas_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;        
        }        

        private async Task<EmpresasRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);
            await PopulateDataAsync(context);
            return new EmpresasRepository(context);
        }

        [Fact]
        public async Task ObtenerEmpresas_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var empresas = repository.ObtenerEmpresas();
            var result = Assert.IsType<List<Empresa>>(empresas?.Result.Value);

            //Assert
            Assert.Equal("Daka", result[0].Nombre);            
        }
        [Fact]
        public async Task ObtenerEmpresasPorIdProfesional_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            var idProfesional = 1;
            //Act
            var empresas = repository.ObtenerEmpresasPorIdProfesional(idProfesional);
            var result = Assert.IsType<List<Empresa>>(empresas?.Result.Value);

            //Assert
            Assert.Equal("Daka", result[0].Nombre);
        }


        [Fact]
        public async Task ObtenerEmpresaPorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var empresa = repository.ObtenerEmpresaPorId(1);
            var result = empresa?.Result.Value;

            //Assert            
            Assert.Equal("Daka", result?.Nombre);            
        }

        [Fact]
        public async Task ObtenerEmpresaPorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var empresa = repository.ObtenerEmpresaPorId(10);
            var result = empresa?.Result.Errors;

            //Assert            
            Assert.True(empresa?.Result.IsError);
            Assert.Equal("Empresa.NoEncontrada", result?[0].Code);
            Assert.Equal("Empresa no encontrada", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarEmpresa_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "Nombre empresa cambio" };
            var repository = await CreateRepositoryAsync();

            var empresa = await repository.ObtenerEmpresaPorId(id);
            empresa.Value.Nombre = "Nombre empresa";

            //Act
            var empresaActualizada = await repository.ActualizarEmpresa(empresa.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Nombre empresa", empresaActualizada.Value.Nombre);
        }

        [Fact]
        public async Task ActualizarEmpresa_NoEncontrada()
        {
            var listaCambios = new List<string> { "Beco" };
            var empresaActualizada = new Empresa { Id = 999, Nombre = "Nombre empresa" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarEmpresa(empresaActualizada, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Empresa.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Empresa no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task CrearEmpresa_Ok()
        {
            var empresa = new Empresa()
            {
                Id = 14,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
            };

            var repository = await CreateRepositoryAsync();

            //Act
            var empresaNueva = await repository.CrearEmpresa(empresa);

            //Assert
            var result = Assert.IsType<ErrorOr<Empresa>>(empresaNueva);
            Assert.Equal(14, result.Value.Id);
            Assert.Equal("Daka", result.Value.Nombre);            
        }

        [Fact]
        public async Task EliminarEmpresa_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarEmpresa(999);
            Assert.True(error.IsError);
            Assert.Equal("Empresa.NoEncontrada", error.Errors[0].Code);
            Assert.Equal("Empresa no encontrada", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarEmpresa_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarEmpresa(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task EmpresaDuplicada_DevuelveTrue()
        {

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


            // Arrange
            var empresa = new Empresa()
            {
                Id = 2,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = profesional.Id,
                Profesional = profesional
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarEmpresa(empresa.Id,empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task EmpresaDuplicada_DevuelveFalse()
        {

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


            // Arrange
            var empresa = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123451",
                IdTipoIdentificacion = 'F',
                IdProfesional = profesional.Id,
                Profesional = profesional
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task EmpresaDuplicada_MismoDevuelveFalse()
        {

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


            // Arrange
            var empresa = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = profesional.Id,
                Profesional = profesional
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional);
            Assert.False(result.Value);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
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

            var empresa1 = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = profesional
            };

            var empresa2 = new Empresa()
            {
                Id = 2,
                Nombre = "CAF",
                NumeroIdentificacion = "123457",
                IdTipoIdentificacion = 'F',
                Profesional = profesional
            };

            var empresa3 = new Empresa()
            {
                Id = 3,
                Nombre = "Makro",
                NumeroIdentificacion = "123458",
                IdTipoIdentificacion = 'F',
                Profesional = profesional
            };
            
            context.Empresas.Add(empresa1);
            context.Empresas.Add(empresa2);
            context.Empresas.Add(empresa3);
            context.Profesionales.Add(profesional);

            await context.SaveChangesAsync();
        }

    }
}
