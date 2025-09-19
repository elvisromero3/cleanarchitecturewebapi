using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.CrearCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EditarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarCliente;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.EliminarClientes;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos;

using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientePorId;
using VUCE3.Catalogos.Aplicacion.Clientes.Queries.ObtenerClientes;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;


namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class ClientesControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerClientes_OK()
        {
            //Arange
            var lstClientes = new List<Cliente>()
            {
                new Cliente()
                {
                    Id =1,
                    CodigoCliente = "12345",
                    NombreCliente ="Pedro",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento =new DateTime(2025, 10, 10)
                },
                new Cliente()
                {
                    Id =2,
                    CodigoCliente = "12345",
                    NombreCliente ="Juan",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento = new DateTime(2025, 10, 10)
                },
                new Cliente()
                {
                     Id = 3,
                    CodigoCliente = "12345",
                    NombreCliente = "Luisa",
                    NumeroIdentificacion = "123245678",
                    IdTipoIdentificacion = 'F',
                    FechaVencimiento =new DateTime(2025, 10, 10)
                }
            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerClientesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstClientes);

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ClienteDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ClienteDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("12345", querydto[0].CodigoCliente);
            Assert.Equal("Pedro", querydto[0].NombreCliente);
            Assert.Equal("123245678", querydto[0].NumeroIdentificacion);           
            Assert.Equal('F', querydto[0].IdTipoIdentificacion);
        }

        [Fact]
        public async Task ObtenerClientes_Error()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerClientesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());           

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerClientePorId_Ok()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerClientePorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ClienteDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ClienteDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("12345", querydto.CodigoCliente);
            Assert.Equal("Pedro", querydto.NombreCliente);
            Assert.Equal("123245678", querydto.NumeroIdentificacion);
            Assert.Equal('F', querydto.IdTipoIdentificacion);
        }

        [Fact]
        public async Task ObtenerClientePorId_Error()
        {
            var error = ErroresCliente.NoEncontrado;                       

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerClientePorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerClientePorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCliente_OK()
        {
            //Arrange
            var cliente = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var clienteDto = new ClienteDto()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

            var controller = new ClientesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(clienteDto);

            var okResult = Assert.IsType<CreatedODataResult<ClienteDto>>(result);
            var resultCliente = Assert.IsType<ClienteDto>(okResult.Value);
            Assert.Equal(1, resultCliente.Id);
            Assert.Equal("12345", resultCliente.CodigoCliente);
            Assert.Equal("Pedro", resultCliente.NombreCliente);
            Assert.Equal("123245678", resultCliente.NumeroIdentificacion);
            Assert.Equal('F', resultCliente.IdTipoIdentificacion);
            Assert.Equal(new DateTime(2025, 10, 10), resultCliente.FechaVencimiento);
        }

        [Fact]
        public async Task PostCliente_DevuelveBadRequest()
        {
            var cliente = new Cliente()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var clienteDto = new ClienteDto()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);           

            var controller = new ClientesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(clienteDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCliente_DevuelveProblem()
        {
            var clienteDto = new ClienteDto()
            {
                Id = 1,
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                IdTipoIdentificacion = 'F',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var cliente = Error.Failure();            

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);            

            var controller = new ClientesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(clienteDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchCliente_InvalidModel()
        {
            var mockSender = new Mock<ISender>();           

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ClienteDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCliente_Ok()
        {
            var grupo = new Cliente()
            {
                Id = 32,
                NombreCliente = "b"
            };
            var grupoDto = new ClienteDto()
            {
                Id = 32,
                NombreCliente = "a"
            };

            var delta = new Delta<DTO.ClienteDto>(grupoDto.GetType());
            delta.TrySetPropertyValue(nameof(grupoDto.NombreCliente), "b");
            delta.TrySetPropertyValue(nameof(grupoDto.IdTipoIdentificacion), 1);
            delta.TrySetPropertyValue(nameof(grupoDto.NumeroIdentificacion), "123456789");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarClienteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(grupo, grupo));

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchCliente_NotFound()
        {
            var error = ErroresCliente.NoEncontrado;
            
            var grupoDto = new ClienteDto()
            {
                Id = 32,
                NombreCliente = "a"
            };            

            var delta = new Delta<ClienteDto>(grupoDto.GetType());
            delta.TrySetPropertyValue(nameof(grupoDto.Id), 1);
            delta.TrySetPropertyValue(nameof(grupoDto.CodigoCliente), 12345);
            delta.TrySetPropertyValue(nameof(grupoDto.NombreCliente), "Pedro");
            delta.TrySetPropertyValue(nameof(grupoDto.NumeroIdentificacion), "123245678");
            delta.TrySetPropertyValue(nameof(grupoDto.IdTipoIdentificacion), 1);
            delta.TrySetPropertyValue(nameof(grupoDto.FechaVencimiento), new DateTime(2025, 10, 10));

            var mockSender = new Mock<ISender>();            
            
            mockSender.Setup(s => s.Send(It.IsAny<EditarClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new ClientesController(mockSender.Object,AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);            

            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteCliente_NotFound()
        {
            var error = ErroresCliente.NoEncontrado;            

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteCliente_Ok()
        {
            var cliente = new Cliente()
            {
                Id = 32,
                NombreCliente = "b"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarClienteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(32);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCliente_InvalidModel()
        {
            var mockSender = new Mock<ISender>();           

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var importaclienteDto = new ImportarClienteDto()
            {
  
                CodigoCliente = "12345",
                NombreCliente = "Pedro",
                NumeroIdentificacion = "123245678",
                TipoIdentificacion = 'D',
                FechaVencimiento = new DateTime(2025, 10, 10)
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarClienteDto>() { importaclienteDto});

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);
          
            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroEstructuraError()
        {            
            IEnumerable<object> objetos = new List<object>
            {
                new CultivoDto { Id = 1, Nombre = "Nombre 3", NombreCientifico = "N3" },
                new CultivoDto { Id = 2, Nombre = "Nombre 4", NombreCientifico = "N4" }
            };

            IEnumerable<ImportarClienteDto> clientes = objetos.Cast<ImportarClienteDto>();           

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", clientes);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);            

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarClienteDto>());           

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveErrorModeloInvalido()
        {
            //Arrange           

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }


        [Fact]
        public async Task EliminarClientes_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarClientesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarClientes_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });           

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarClientesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarClientes_DevuelveErrorModeloInvalido()
        {
            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostCliente_DevuelveInvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ClienteDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchCliente_DevuelveInvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new ClientesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ClienteDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarClienteDto { CodigoCliente = "123" }
            };

            IEnumerable<ImportarClienteDto> clientes = objetos.Cast<ImportarClienteDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", clientes);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new ClientesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
