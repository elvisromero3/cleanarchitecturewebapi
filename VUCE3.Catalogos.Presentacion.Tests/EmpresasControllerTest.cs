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
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresaPorId;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.CrearEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EditarEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresa;
using Microsoft.AspNetCore.OData.Formatter;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class EmpresasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerEmpresas_OK()
        {
            //Arange
            var lstEmpresas = new List<Empresa>()
            {
               new Empresa()
                {
                    Id = 1,
                    Nombre = "Daka",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Profesional { Id = 1}
                },

                new Empresa()
                {
                    Id = 2,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Profesional { Id = 1}
                },

                new Empresa()
                {
                    Id = 3,
                    Nombre = "Makro",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Profesional { Id = 1}
                }

            };

            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEmpresasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(lstEmpresas);
            
            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<List<EmpresaDto>>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<EmpresaDto>>(queryResult.Value);    
            Assert.Equal(1, querydto[0].Id);
            Assert.Equal("Daka", querydto[0].Nombre);            
            
        }

        [Fact]
        public async Task ObtenerEmpresas_Error()
        {
            // Error
            var error = ErroresEmpresas.NoEncontradas;            

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEmpresasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get();

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            

        }

        [Fact]
        public async Task ObtenerEmpresaPorId_Ok()
        {
            //Arrange
            var empresa = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Profesional { Id = 1 }
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEmpresaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(empresa);

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<EmpresaDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<EmpresaDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Daka", querydto.Nombre);
        }

        [Fact]
        public async Task ObtenerEmpresaPorId_Error()
        {
            var error = ErroresEmpresas.NoEncontrado;           


            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerEmpresaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);            
        }

        [Fact]
        public async Task ObtenerEmpresaPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PostEmpresa_DevuelveCreated()
        {
            var empresaDto = new EmpresaDto()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            var empresa = new Empresa()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Profesional { Id = 1 }
            };



            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearEmpresaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(empresa);

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(empresaDto);

            var okResult = Assert.IsType<CreatedODataResult<EmpresaDto>>(result);
            var resultEmpresa = Assert.IsType<EmpresaDto>(okResult.Value);
            Assert.Equal("Daka", resultEmpresa.Nombre);            
        }

        [Fact]
        public async Task PostEmpresa_DevuelveModelInvalid()
        {
            var mockSender = new Mock<ISender>();

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Post(new EmpresaDto());
            Assert.IsType<ODataErrorResult>(result);            
        }

        [Fact]
        public async Task PostEmpresa_DevuelveProblem()
        {
            var empresaDto = new EmpresaDto()
            {
                Id = 1,
                Nombre = "Daka",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            var empresa = Error.Failure();

            //Mock para Mediator
            Mock<ISender> mockMediator = new Mock<ISender>();
            mockMediator.Setup(m => m.Send(It.IsAny<CrearEmpresaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(empresa);            

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Post(empresaDto);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task PatchEmpresa_InvalidModel()
        {            

            var mockSender = new Mock<ISender>();

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Patch(1, new Delta<EmpresaDto>());
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task PatchEmpresa_Ok()
        {
            var empresa = new Empresa()
            {
                Id = 32,
                Nombre = "Empresa b"
            };

            var empresaDto = new EmpresaDto()
            {
                Id = 32,
                Nombre = "a"
            };

            var delta = new Delta<EmpresaDto>(empresaDto.GetType());
            delta.TrySetPropertyValue(nameof(empresaDto.Nombre), "Empresa b");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarEmpresaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(empresa, empresa));

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PatchEmpresa_NotFound()
        {
            var error = ErroresEmpresas.NoEncontrado;            

            var empresaDto = new EmpresaDto()
            {
                Id = 32,
                Nombre = "Empresa a"
            };

            var delta = new Delta<EmpresaDto>(empresaDto.GetType());
            delta.TrySetPropertyValue(nameof(empresaDto.Nombre), "Empresa b");

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EditarEmpresaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Patch(1, delta);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteEmpresa_NotFound()
        {
            var error = ErroresEmpresas.NoEncontrado;           

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarEmpresaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteEmpresa_Ok()
        {
            var empresa = new Empresa()
            {
                Id = 32,
                Nombre = "Empresa b"
            };

            var mockRepo = new Mock<IUnitOfWork>();

            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<EliminarEmpresaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(empresa);

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Delete(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEmpresa_InvalidModel()
        {
            var mockSender = new Mock<ISender>();

            var controller = new EmpresasController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Delete(1);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange

            var importarEmpresaDto = new ImportarEmpresaDto()
            {
                IdProfesional=2,
                Nombre="Home Depot",
                NumeroIdentificacion= "TRD4",
                TipoIdentificacion = 'D'
            };

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarEmpresaDto>() { importarEmpresaDto });

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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

            IEnumerable<ImportarEmpresaDto> empresas = objetos.Cast<ImportarEmpresaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", empresas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);            

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarEmpresaDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

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
            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarEmpresass_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarEmpresasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarEmpresas_DevuelveError()
        {           

            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarEmpresasCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());         

            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarEmpresas_DevuelveErrorModeloInvalido()
        {
            var controller = new EmpresasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("items", "El campo items es requerido");

            var result = await controller.BorradoMasivo(new ODataActionParameters());

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_FicheroDatosNullError()
        {
            //Arrange
            IEnumerable<object> objetos = new List<object>
            {
                new ImportarEmpresaDto { Nombre = "Test" }
            };

            IEnumerable<ImportarEmpresaDto> empresas = objetos.Cast<ImportarEmpresaDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", empresas);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new EmpresasController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

    }
}
