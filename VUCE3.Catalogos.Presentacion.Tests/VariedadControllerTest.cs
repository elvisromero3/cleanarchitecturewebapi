using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EditarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedadPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Presentacion.Tests.Helper;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class VariedadControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerVariedades_DevuelveOk()
        {
            //Arrange
            var variedades = new List<Variedad>
            {
                new Variedad
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre1"
                },
                new Variedad
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Nombre2"
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerVariedadesQuery>(), default)).ReturnsAsync(variedades);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var variedadesDto = Assert.IsType<List<VariedadDto>>(okResult.Value);

            Assert.Equal(variedades.Count, variedadesDto.Count);

            for (int i = 0; i < variedades.Count; i++)
            {
                Assert.Equal(variedades[i].Id, variedadesDto[i].Id);
                Assert.Equal(variedades[i].Codigo, variedadesDto[i].Codigo);
                Assert.Equal(variedades[i].Nombre, variedadesDto[i].Nombre);
            }
        }

        [Fact]
        public async Task ObtenerVariedades_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;           

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerVariedadesQuery>(), default)).ReturnsAsync(error);            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerVariedadPorId_DevuelveOk()
        {
            //Arrange
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerVariedadPorIdQuery>(), default)).ReturnsAsync(variedad);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var variedadDto = Assert.IsType<VariedadDto>(okResult.Value);

            Assert.Equal(variedad.Id, variedadDto.Id);
            Assert.Equal(variedad.Codigo, variedadDto.Codigo);
            Assert.Equal(variedad.Nombre, variedadDto.Nombre);
        }

        [Fact]
        public async Task ObtenerVariedadPorId_NoExiste_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;            

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerVariedadPorIdQuery>(), default)).ReturnsAsync(error);            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerVariedadPorId_DevuelveErrorModeloInvalido()
        {
            //Arrange
            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
            controller.ModelState.AddModelError("Id", "El campo Id es requerido");            

            //Act
            var result = await controller.Get(0);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveCreated()
        {
            //Arrange
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<CrearVariedadCommand>(), default)).ReturnsAsync(variedad);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Post(variedadDto);

            //Assert
            var okResult = Assert.IsType<CreatedODataResult<VariedadDto>>(result);
            var resultVariedad = Assert.IsType<VariedadDto>(okResult.Value);
            Assert.Equal("Nombre1", resultVariedad.Nombre);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;

            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };            

            mockMediator.Setup(m => m.Send(It.IsAny<CrearVariedadCommand>(), default)).ReturnsAsync(error);            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Post(variedadDto);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }


        [Fact]
        public async Task CrearVariedad_DevuelveModelInvalid()
        {
            //Arrange            
            
            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
            controller.ModelState.AddModelError("Codigo", "El campo Codigo es requerido");

            //Act
            var result = await controller.Post(variedadDto);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EditarVariedad_DevuelveOk()
        {
            //Arrange
            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            var delta = new Delta<VariedadDto>(variedadDto.GetType());
            delta.TrySetPropertyValue(nameof(variedadDto.Nombre), "Nombre1");

            var mockRepo = new Mock<IUnitOfWork>();

            mockMediator.Setup(s => s.Send(It.IsAny<EditarVariedadCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Tuple.Create(variedad, variedad));

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Patch(variedadDto.Id.Value, delta);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditarVariedad_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;            

            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };
            var delta = new Delta<VariedadDto>(variedadDto.GetType());
            delta.TrySetPropertyValue(nameof(variedadDto.Nombre), "Nombre2");

            mockMediator.Setup(m => m.Send(It.IsAny<EditarVariedadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Patch(variedadDto.Id.Value, delta);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EditarVariedad_NoExiste_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;

            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };
            var delta = new Delta<VariedadDto>(variedadDto.GetType());
            delta.TrySetPropertyValue(nameof(variedadDto.Nombre), "Nombre2");

            mockMediator.Setup(m => m.Send(It.IsAny<EditarVariedadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Patch(variedadDto.Id.Value, delta);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }


        [Fact]
        public async Task EditarVariedad_DevuelveModelInvalid()
        {
            //Arrange            

            var variedadDto = new VariedadDto
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };
            var delta = new Delta<VariedadDto>(variedadDto.GetType());
            delta.TrySetPropertyValue(nameof(variedadDto.Nombre), "Nombre2");

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
            controller.ModelState.AddModelError("Codigo", "El campo Codigo es requerido");

            //Act
            var result = await controller.Patch(variedadDto.Id.Value, delta);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarVariedad_DevuelveOk()
        {
            //Arrange
            var id = 1;
            
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Nombre1"
            };

            EliminarVariedadCommand _eliminarVariedadCommand = new EliminarVariedadCommand { IdVariedad = id };
            mockMediator.Setup(mediator => mediator.Send(_eliminarVariedadCommand, default)).ReturnsAsync(variedad);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Delete(id);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EliminarVariedad_DevuelveError()
        {
            //Arrange
            var error = ErroresVariedad.NoEncontrado;

            var id = 1;            

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarVariedadCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);           

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.Delete(id);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarVariedad_NoExiste_DevuelveErrorModeloInvalido()
        {
            //Arrange
            var id = 1;

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
            controller.ModelState.AddModelError("Id", "El campo Id es requerido");            

            //Act
            var result = await controller.Delete(id);

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOk()
        {
            //Arrange
            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", new List<ImportarVariedadDto>());

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

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

            IEnumerable<ImportarVariedadDto> variedades = objetos.Cast<ImportarVariedadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 1);
            param.Add("datos", variedades);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

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
            param.Add("datos", new List<ImportarVariedadDto>());            

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

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
            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
            controller.ModelState.AddModelError("Modo", "El campo Modo es requerido");            

            //Act
            var result = await controller.ImportarDatos(new ODataActionParameters());

            //Assert
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task EliminarVariedades_DevuelveOk()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarVariedadesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Deleted);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            //Act
            var result = await controller.BorradoMasivo(param);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarVariedades_DevuelveError()
        {
            var param = new ODataActionParameters();
            param.Add("items", new List<int> { 1, 2, 3 });            

            mockMediator.Setup(m => m.Send(It.IsAny<EliminarVariedadesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.Failure());
            

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);

            var result = await controller.BorradoMasivo(param);

            //Assert
            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("500", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task EliminarVariedades_DevuelveErrorModeloInvalido()
        {   

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper,mockLocalizer.Object);
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
                new ImportarVariedadDto { Codigo = "123", Nombre = "Variedad" }
            };

            IEnumerable<ImportarVariedadDto> variedades = objetos.Cast<ImportarVariedadDto>();

            var param = new ODataActionParameters();
            param.Add("modo", 2);
            param.Add("datos", variedades);

            mockMediator.Setup(m => m.Send(It.IsAny<ImportarDatosCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Created);

            var controller = new VariedadController(mockMediator.Object, AutomapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.ImportarDatos(param);

            //Assert
            Assert.IsType<CreatedResult>(result);
        }

    }
}
