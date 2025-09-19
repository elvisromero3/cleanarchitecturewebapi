using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class ClientesRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;
        public ClientesRepositoryTest()
        {
            var dbName = $"BDTestClientes_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        private async Task<ClientesRepository> CreateRepositoryAsync()
        {
            CatalogosDbContext context = new CatalogosDbContext(dbContextOptions);

            await PopulateDataAsync(context);
            return new ClientesRepository(context);
        }

        [Fact]
        public async Task ObtenerClientes_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var clientes = repository.ObtenerClientes();
            var result = Assert.IsType<List<Cliente>>(clientes?.Result.Value);

            //Assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("12345", result[0].CodigoCliente);
            Assert.Equal("Pedro", result[0].NombreCliente);
            Assert.Equal("123245678", result[0].NumeroIdentificacion);
            Assert.Equal('F', result[0].IdTipoIdentificacion);
            Assert.Equal(new DateTime(2025, 10, 10), result[0].FechaVencimiento);
        }

        [Fact]
        public async Task ObtenerClientePorId_Ok()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var clientes = repository.ObtenerClientePorId(1);
            var result = clientes?.Result.Value;

            //Assert
            Assert.False(clientes?.Result.IsError);
            Assert.Equal(1, result?.Id);
            Assert.Equal("12345", result?.CodigoCliente);
            Assert.Equal("Pedro", result?.NombreCliente);
            Assert.Equal("123245678", result?.NumeroIdentificacion);
            Assert.Equal('F', result?.IdTipoIdentificacion);
            Assert.Equal(new DateTime(2025, 10, 10), result?.FechaVencimiento);
        }

        [Fact]
        public async Task ObtenerClientePorId_Error()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();

            //Act
            var cliente = repository.ObtenerClientePorId(10);
            var result = cliente?.Result.Errors;

            //Assert            
            Assert.True(cliente?.Result.IsError);
            Assert.Equal("Cliente.NoEncontrado", result?[0].Code);
            Assert.Equal("Cliente no encontrado", result?[0].Description);
        }

        [Fact]
        public async Task ActualizarCliente_Ok()
        {
            //Arrange
            int id = 1;
            var listaCambios = new List<string> { "NombreCliente" };
            var repository = await CreateRepositoryAsync();

            var cliente = await repository.ObtenerClientePorId(id);
            cliente.Value.NombreCliente = "Nombre cliente";

            //Act
            var clienteActualizada = await repository.ActualizarCliente(cliente.Value, 1, listaCambios);

            //Assert
            Assert.Equal("Nombre cliente", clienteActualizada.Value.NombreCliente);
        }

        [Fact]
        public async Task ActualizarCliente_NoEncontrada()
        {
            var listaCambios = new List<string> { "CodigoCliente" };
            var clienteActualizado = new Cliente { Id = 999, CodigoCliente = "123", NombreCliente = "Nombre cliente" };

            var repository = await CreateRepositoryAsync();

            //Act
            var error = await repository.ActualizarCliente(clienteActualizado, 999, listaCambios);
            Assert.True(error.IsError);
            Assert.Equal("Cliente.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Cliente no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task CrearCliente_Ok()
        {
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var repository = await CreateRepositoryAsync();

            //Act
            var clienteNuevo = await repository.CrearCliente(cliente);

            //Assert
            var result = Assert.IsType<ErrorOr<Cliente>>(clienteNuevo);
            Assert.Equal(15, result.Value.Id);
            Assert.Equal("12345", result.Value.CodigoCliente);
            Assert.Equal("Pedro", result.Value.NombreCliente);
            Assert.Equal("123245678", result.Value.NumeroIdentificacion);
            Assert.Equal('F', result.Value.IdTipoIdentificacion);
            Assert.Equal(new DateTime(2025, 10, 10), result.Value.FechaVencimiento);
        }

        [Fact]
        public async Task EliminarCliente_NoEncontrada()
        {
            var repository = await CreateRepositoryAsync();

            var error = await repository.EliminarCliente(999);
            Assert.True(error.IsError);
            Assert.Equal("Cliente.NoEncontrado", error.Errors[0].Code);
            Assert.Equal("Cliente no encontrado", error.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarCliente_OK()
        {
            var repository = await CreateRepositoryAsync();
            var error = await repository.EliminarCliente(1);

            Assert.False(error.IsError);
        }

        [Fact]
        public async Task ClienteDuplicado_DevuelveTrue()
        {

            // Arrange
            var cliente = new Cliente()
            {
                Id = 2,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarCliente(cliente.Id, cliente.CodigoCliente,cliente.NombreCliente,cliente.IdTipoIdentificacion,cliente.NumeroIdentificacion,cliente.FechaVencimiento);
            Assert.True(result.Value);            
        }

        [Fact]
        public async Task ClienteDuplicado_DevuelveFalse()
        {

            // Arrange
            var cliente = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12340",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ClienteDuplicado_MismoDevuelveFalse()
        {

            // Arrange
            var cliente = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var repository = await CreateRepositoryAsync();

            var result = await repository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento);
            Assert.False(result.Value);
        }

        private static async Task PopulateDataAsync(CatalogosDbContext context)
        {
            var cliente1 = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var cliente2 = new Cliente()
            {
                Id = 2,
                CodigoCliente = "12346",
                NombreCliente = "Paco",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var cliente3 = new Cliente()
            {
                Id = 3,
                CodigoCliente = "12347",
                NombreCliente = "Lucas",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            context.Clientes.Add(cliente1);
            context.Clientes.Add(cliente2);
            context.Clientes.Add(cliente3);

            await context.SaveChangesAsync();
        }
    }
}
