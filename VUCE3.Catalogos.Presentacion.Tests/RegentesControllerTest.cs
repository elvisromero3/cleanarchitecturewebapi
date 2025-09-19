using Moq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;
using Microsoft.AspNetCore.OData.Deltas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionalPorId;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.CrearProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EditarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Presentacion.Tests.Helper;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class RegentesControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerProfesionales_OK()
        {
            //Arange
            var lstProfesianales = new List<Profesional>()
            {
                new Profesional()
                {
                    Id = 1,
                    Nombre = "Pedro Perez",
                    IdTipoIdentificacion = 'F',
                    NumeroIdentificacion = "123456",
                    Profesion = "Ingeniero",
                    Email = "test@test.com",
                    CodigoRegente = "123456",
                    Activo = true
                },
                new Profesional()
                {
                    Id = 2,
                    Nombre = "Pedro Sanchez",
                    IdTipoIdentificacion = 'F',
                    NumeroIdentificacion = "123456",
                    Profesion = "Comerciante",
                    Email = "test@test.com",
                    CodigoRegente = "123456",
                    Activo = true
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProfesionalesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstProfesianales);
            
            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<ProfesionalDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProfesionalDto>>(queryResult.Value);
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Pedro Perez", querydto[0].Nombre);            
            
        }

        [Fact]
        public async Task ObtenerProfesionales_Error()
        {
            // Error
            var error = ErroresProfesionales.NoEncontradas;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProfesionalesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);
            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerProfesionalPorId_Ok()
        {
            //Arrange
            
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

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProfesionalPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(profesional);

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<ProfesionalDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ProfesionalDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Pedro Perez", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerProfesionalPorId_Error()
        {
            var error = ErroresProfesionales.NoEncontrado;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerProfesionalPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task ObtenerProfesionalPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostProfesional_DevuelveBadRequest()
        {
            var profesionalDto = new ProfesionalDto()
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

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();

            CrearProfesionalCommand crearProfesional = new CrearProfesionalCommand()
            {
                Profesional = profesional
            };

            mockMediator.Setup(m => m.Send(crearProfesional, It.IsAny<CancellationToken>())).ReturnsAsync(profesional);            

            var controller = new RegentesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("action", "test");

            var result = await controller.Post(profesionalDto);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostProfesional_DevuelveCreated()
        {
            var profesionalDto = new ProfesionalDto()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion = 1
            };

            var profesional = new Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion = 1,
                Empresas = new List<Empresa>()
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProfesionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(profesional);

            var controller = new RegentesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(profesionalDto);

            Assert.IsType<CreatedODataResult<ProfesionalDto>>(result);
        }

        [Fact]
        public async Task PostProfesional_DevuelveProblem()
        {
            var profesionalDto = new ProfesionalDto()
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

            var profesional = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearProfesionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(profesional);            

            var controller = new RegentesController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(profesionalDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchProfesional_InvalidModel()
        {            

            var mockSender = new Mock<ISender>();            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ProfesionalDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchProfesional_Ok()
        {
            var profesional = new Profesional()
            {
                Id = 1,
                Nombre = "Profesional a"
            };

            var profesionalDto = new ProfesionalDto()
            {
                Id = 1,
                Nombre = "Profesional b"
            };

            var delta = new Delta<ProfesionalDto>(profesionalDto.GetType());
            delta.TrySetPropertyValue(nameof(profesionalDto.Nombre), "Profesional a");
            delta.TrySetPropertyValue(nameof(profesionalDto.IdTipoIdentificacion), 1);
            delta.TrySetPropertyValue(nameof(profesionalDto.NumeroIdentificacion), "123456789");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProfesionalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(profesional, profesional));

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchProfesional_NotFound()
        {
            var error = ErroresProfesionales.NoEncontrado;

            var profesionalDto = new ProfesionalDto()
            {
                Id = 1,
                Nombre = "Profesional a"
            };            

            var delta = new Delta<ProfesionalDto>(profesionalDto.GetType());
            delta.TrySetPropertyValue(nameof(profesionalDto.Nombre), "Profesional b");
            delta.TrySetPropertyValue(nameof(profesionalDto.IdTipoIdentificacion), 1);
            delta.TrySetPropertyValue(nameof(profesionalDto.NumeroIdentificacion), "123456789");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarProfesionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteProfesional_NotFound()
        {
            var error = ErroresProfesionales.NoEncontrado;           

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProfesionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task DeleteProfesional_Ok()
        {
            var profesional = new Profesional()
            {
                Id = 1,
                Nombre = "Profesional a"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarProfesionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(profesional);

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProfesional_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarProfesionalDto>
            {
                new ImportarProfesionalDto(){
                Nombre = "A",
                Activo  = true,
                TipoIdentificacion = 'D',
                NumeroIdentificacion = "3RTFF",
                Profesion ="Consultor",
                Email = "procomer@procomer.com",
                CodigoRegente ="ER4",
                Institucion = "MINCEX",
                IdInstitucion =-1
            } });

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroEstructuraError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new CultivoDto { Id = 1, Nombre = "Nombre 1", NombreCientifico = "N1" },
                new CultivoDto { Id = 2, Nombre = "Nombre 2", NombreCientifico = "N2" }
            };            

            IEnumerable<ImportarProfesionalDto> profesionales = objetos.Cast<ImportarProfesionalDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", profesionales);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);            

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarProfesionalDto>());            

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

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

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarProfesionales_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProfesionalesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarProfesionales_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });            

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarProfesionalesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarProfesionales_DevuelveErrorModeloInvalido()
        {
            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchProfesionales_DevuelveInvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new RegentesController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<ProfesionalDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarProfesionalDto { Nombre = "Test" }
            };

            IEnumerable<ImportarProfesionalDto> profesionales = objetos.Cast<ImportarProfesionalDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", profesionales);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new RegentesController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
