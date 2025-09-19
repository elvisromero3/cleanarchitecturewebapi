using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.CrearCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EditarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarClientes;
using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientePorId;
using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientes;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ClientesTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
           
        public ClientesTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerClientes_Ok()
        {
            //Act
            var clientes = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            ObtenerClientesQuery obtenerClientesQuery = new ObtenerClientesQuery();
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(clientes);
            var handler = new ObtenerClientesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerClientesQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Cliente>>(result.Value);
        }

        [Fact]
        public async Task ObtenerClientePorId_Ok()
        {
            //Arange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            //Act
            ObtenerClientePorIdQuery obtenerClientePorIdQuery = new ObtenerClientePorIdQuery();
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(1)).ReturnsAsync(cliente);
            var handler = new ObtenerClientePorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerClientePorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_Ok()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(15, result.Value.Id);
            Assert.Equal("12345", result.Value.CodigoCliente);
            Assert.Equal("123245678", result.Value.NumeroIdentificacion);
            Assert.Equal("Pedro", result.Value.NombreCliente);
            Assert.Equal('F', result.Value.IdTipoIdentificacion);
        }

        [Fact]
        public async Task CrearCliente_ErrorCodigoCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "1234567890123456789012345678901234567890",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_ErrorCodigoClienteVacio()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoCodigoCliente", result.Errors[0].Code);
            Assert.Equal("El código de cliente es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 35 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearCliente_ErrorTipoIdentificacion()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'X',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_ErrorNombreClienteVacio()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "abc",
                NombreCliente = "",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNombreCliente", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearCliente_ErrorNumeroIdentificacionVacio()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "abc",
                NombreCliente = "Nombre",
                NumeroIdentificacion = "",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNroIdentificacion", result.Errors[0].Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.Errors[0].Description);
        }
        [Fact]
        public async Task CrearCliente_ErrorTamanoNroIdentificacionFisica()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "12324567890",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_ErrorNroIdentificacionFisicaComienza0()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "023456789",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_ErrorTamanoNroIdentificacionOtras()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "12324567890000000",
                IdTipoIdentificacion = 'P',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_ErrorTamanoNroIdentificacionDimex()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "12324567890000000",
                IdTipoIdentificacion = 'D',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCliente_ErrorNombreCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro Picapiedra Pedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro Picapiedra",
                NumeroIdentificacion = "12324560",
                IdTipoIdentificacion = 'P',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorFechasVencimiento()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "12324560",
                IdTipoIdentificacion = 'P',
                FechaVencimiento = new DateTime(0001, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ExisteCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678A",
                NombreCliente = "Pedro Picapiedra",
                NumeroIdentificacion = "12324560",
                IdTipoIdentificacion = 'P',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);

            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCliente_Error()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var error = Error.Unexpected();

            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(error);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.FirstError.Code);
            Assert.Equal("An unexpected error has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_Ok()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var listaCambios = new List<string> { "NombreCliente", "FechaVencimiento" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCliente_NoExiste()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "NombreCliente" };
            var errorIsError = ErroresCliente.NoEncontrado;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cliente no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorIdTipoIdentificacion()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'X',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "IdTipoIdentificacion" };
            var errorIsError = ErroresCliente.NoEncontrado;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.tipoIdentificacionNoEncontrado", result.FirstError.Code);
            Assert.Equal("Tipo Identificacion no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorTipoIdentificacionNumeroIdentificacion()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "1232456785",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "IdTipoIdentificacion", "NumeroIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorNumeroIdentificacion()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "1232456785",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "NumeroIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorCodigoCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345678901235698753214569852A4555555485555555",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "12456785",
                IdTipoIdentificacion = 'P',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "CodigoCliente" };

            //Act

            var error = ErroresCliente.ClienteTamanoCodigoCliente;

            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
            
        }

        [Fact]
        public async Task ActualizarCliente_ErrorNombreCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro Picapiedra",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "NombreCliente" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNombreCliente", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorTamanoNumeroidentificacionDimex()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro PicapiedraPedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'D',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "NumeroIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorFechaVencimiento()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro PicapiedraPedro Picapiedra",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(1, 10, 10)
            };
            var listaCambios = new List<string> { "FechaVencimiento" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.FechaVencimientoMenorHoy", result.FirstError.Code);
        }

        [Fact]
        public async Task ActualizarCliente_ExisteCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro Picapiedra",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "CodigoCliente", "IdTipoIdentificacion", "IdTipoIdentificacion", "NombreCliente", "IdTipoIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarCliente_ErrorExisteCliente()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro Picapiedra",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "CodigoCliente", "IdTipoIdentificacion", "IdTipoIdentificacion", "NombreCliente", "IdTipoIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(cliente);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarCliente_Error()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "123456A",
                NombreCliente = "Pedro Picapiedra",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };
            var listaCambios = new List<string> { "CodigoCliente", "IdTipoIdentificacion", "IdTipoIdentificacion", "NombreCliente", "IdTipoIdentificacion" };

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(Error.Unexpected());
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCliente_Ok()
        {
            //Arrange
            var idCliente = 1;

            //Act
            EliminarClienteCommand command = new EliminarClienteCommand();
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(idCliente));
            var handler = new EliminarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCliente_NoExiste()
        {
            //Arrange
            var idCliente = 0;
            var errorIsError = ErroresCliente.NoEncontrado;

            //Act
            EliminarClienteCommand command = new EliminarClienteCommand();
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(idCliente)).ReturnsAsync(errorIsError);
            var handler = new EliminarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cliente no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCliente_Error()
        {
            //Arrange
            var idCliente = 0;
            var errorIsError = ErroresCliente.NoEncontrado;

            //Act
            EliminarClienteCommand command = new EliminarClienteCommand();
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(idCliente)).ReturnsAsync(errorIsError);
            var handler = new EliminarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cliente no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "1232456789",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123456789012",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(It.IsAny<Dominio.Entidades.Cliente>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerVariedadesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarVariedadDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearVariedadDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(It.IsAny<Dominio.Entidades.Cliente>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(It.IsAny<Dominio.Entidades.Cliente>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoIdentificacionDevuelveNull()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {

                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex3"
                }
             };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(It.IsAny<Dominio.Entidades.Cliente>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarClientes_DevuelveOk()
        {
            // Arrange
            var idsClientes = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarClientesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarClientesCommand { IdsClientes = idsClientes }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarClientes_DevuelveError()
        {
            // Arrange
            var idsClientes = new List<int> { 1, 2, 3 };
            var erroror = ErroresCliente.NoEncontrado;

            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(3)).ReturnsAsync(erroror);

            var handler = new EliminarClientesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarClientesCommand { IdsClientes = idsClientes }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarClientesCrear_DevuelveError()
        {
            // Arrange                       

            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            //Act

            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento)).ReturnsAsync(true);

            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(cliente);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cliente.Existe", result.FirstError.Code);
            Assert.Equal("Ya existe un cliente con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerClientesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>() { new Cliente() { IdTipoIdentificacion = 'F', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    NombreCliente = "A",
                    CodigoCliente ="abc",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarClientesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'F', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    NombreCliente = "A",
                    CodigoCliente ="abc",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ClientesRepository.EliminarCliente(1)).ReturnsAsync(Error.Failure());
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTipoIdentificacion()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'X',
                    NumeroIdentificacion = "3RTFF",
                    NombreCliente = "A",
                    CodigoCliente ="abc",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTamañoCodigoCliente()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'P',
                    NumeroIdentificacion = "3RTFF",
                    NombreCliente = "A",
                    CodigoCliente ="ab12313111312312321313213212313123131312313213123131313131313131313c",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTamañoIdentificacionFisica()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "3RTFF",
                    NombreCliente = "A",
                    CodigoCliente ="ABC123",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorIdentificacionFisicaComienza0()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "023456789",
                    NombreCliente = "A",
                    CodigoCliente ="ABC123",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTamañoIdentificacionOtras()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'P',
                    NumeroIdentificacion = "3RTF4589654789F",
                    NombreCliente = "A",
                    CodigoCliente ="ABC123",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTamañoNombreCliente()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now.AddDays(1) } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'P',
                    NumeroIdentificacion = "3RTF4589",
                    NombreCliente = "Pedro PabloPedro PabloPedro PabloPedro PabloPedro PabloPedro PabloPedro PabloPedro PabloPedro PabloPedro Pablo",
                    CodigoCliente ="ABC123",
                    FechaVencimiento = DateTime.Now.AddDays(1)
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorFechaVencimiento()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Cliente>() { new Cliente() { Id = 1, IdTipoIdentificacion = 'P', NumeroIdentificacion = "numero", CodigoCliente = "A123", NombreCliente = "Nombre", FechaVencimiento = DateTime.Now } };
            var datosImportar = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    TipoIdentificacion = 'P',
                    NumeroIdentificacion = "3RTF4589",
                    NombreCliente = "Pedro",
                    CodigoCliente ="ABC123",
                    FechaVencimiento = DateTime.Now
                }
            };
            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarClientesDevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            var clientes = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(clientes);
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(0, "12345", "Pedro", 'D', "123245678", new DateTime(2025, 10, 10))).ReturnsAsync(true);


            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearClienteDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarClienteCommandDto>()
            {
                new ImportarClienteCommandDto()
                {
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    TipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            var clientes = new List<Cliente>()
            {
                new Cliente()
                {
                    Id = 15,
                    CodigoCliente = "12345",
                    NombreCliente = "Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'D',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientes()).ReturnsAsync(clientes);
            mockRepo.Setup(repo => repo.ClientesRepository.ValidarCliente(15, "12345", "Pedro", 'D', "123245678", new DateTime(2025, 10, 10))).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(It.IsAny<Cliente>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        

        [Fact]
        public async Task CrearCliente_DevuelveValidacionCodigoError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345123451234512345123451234512345123451234512345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            var error = ErroresCliente.ClienteTamanoCodigoCliente;


            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(error);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearCliente_DevuelveValidacionNumeroIdentificacionError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678912331212",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            var error = ErroresCliente.TamanoNroIdentificacion;


            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(error);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearCliente_DevuelveValidacionTipoIdentificacionError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245",
                IdTipoIdentificacion = 'A',
                FechaVencimiento = new DateTime(2025, 10, 11)
            };

            var error = ErroresCliente.TipoIdentificacionNoEncontrado;


            //Act
            CrearClienteCommand command = new CrearClienteCommand() { Cliente = cliente };
            mockRepo.Setup(repo => repo.ClientesRepository.CrearCliente(cliente)).ReturnsAsync(error);
            var handler = new CrearClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCliente_DevuelveValidacionCodigoError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "12345123451234512345123451234512345123451234512345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var listaCambios = new List<string> { "CodigoCliente" };

            var error = ErroresCliente.ClienteTamanoCodigoCliente;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCliente_DevuelveValidacionNumeroIdentificacionError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "1234512345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var listaCambios = new List<string> { "CodigoCliente" };

            var error = ErroresCliente.NumeroIdentificacionExcedeLimite;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCliente_DevuelveValidacionNumeroIdentificacionComienza0()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "1234512345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "023456789",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var listaCambios = new List<string> { "NumeroIdentificacion" };

            var error = ErroresCliente.TipoIdentificacionFisicaComienza0;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarClienteDevuelveValidacionTipoIdentificacionError()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 15,
                CodigoCliente = "1234512345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'A',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var listaCambios = new List<string> { "IdTipoIdentificacion" };

            var error = ErroresCliente.TipoIdentificacionNoEncontrado;

            //Act
            EditarClienteCommand command = new EditarClienteCommand(cliente, cliente.Id, listaCambios);

            mockRepo.Setup(repo => repo.ClientesRepository.ObtenerClientePorId(15)).ReturnsAsync(cliente);
            mockRepo.Setup(repo => repo.ClientesRepository.ActualizarCliente(command.Cliente, command.IdCliente, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarClienteCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

    }
}
