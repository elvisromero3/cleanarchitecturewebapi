using MediatR;
using Moq;
using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.Controllers;
using Microsoft.AspNetCore.Mvc;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos;
using Microsoft.AspNetCore.OData.Results;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogosPorIdInstitucion;
using Microsoft.Extensions.Localization;
using VUCE3.Catalogos.Presentacion.Resources;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogoPorId;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class CatalogosControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerCatalogos_DevuelveCatalogosOk()
        {
            var catalogos = new List<Catalogo>
            {
                new Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catalogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = true,
                    AccionControlador = "Coleccion 1",
                },
                new Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catalogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = true,
                    AccionControlador = "Coleccion 2",
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCatalogosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(catalogos);

            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var catalogosDto = Assert.IsType<List<CatalogoDto>>(objectResult.Value);

            Assert.Equal(catalogos.Count, catalogosDto.Count);

            for (int i = 0; i < catalogos.Count; i++)
            {
                Assert.Equal(catalogos[i].Id, catalogosDto[i].Id);
                Assert.Equal(catalogos[i].Alias, catalogosDto[i].Alias);
                Assert.Equal(catalogos[i].Nombre, catalogosDto[i].Nombre);
                Assert.Equal(catalogos[i].VisibleEmpresa, catalogosDto[i].VisibleEmpresa);
                Assert.Equal(catalogos[i].RequiereFirma, catalogosDto[i].RequiereFirma);
                Assert.Equal(catalogos[i].AccionControlador, catalogosDto[i].AccionControlador);
            }
        }

        [Fact]
        public async Task ObtenerCatalogos_DevuelveError()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCatalogosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());           

            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ODataErrorResult>(result);

            Assert.Equal("404", objectResult.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCatalogosPorInstitucion_DevuelveCatalogosOk()
        {
            var catalogos = new List<Catalogo>
            {
                new Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catalogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = true,
                    AccionControlador = "Coleccion 1",
                },
                new Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catalogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = true,
                    AccionControlador = "Coleccion 2",
                }
            };

            var catalogoInstitucion = new List<CatalogoInstitucion>
            {
                new CatalogoInstitucion
                {
                    Id = 1,
                    InstitucionId = 1,
                    CatalogoId = 1
                },
                new CatalogoInstitucion
                {
                    Id = 2,
                    InstitucionId = 1,
                    CatalogoId = 2
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCatalogosPorIdInstitucionQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(catalogos);

            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.ObtenerCatalogosPorInstitucion(1);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var catalogosDto = Assert.IsType<List<CatalogoDto>>(objectResult.Value);

            Assert.Equal(catalogos.Count, catalogosDto.Count);

            for (int i = 0; i < catalogos.Count; i++)
            {
                Assert.Equal(catalogos[i].Id, catalogosDto[i].Id);
                Assert.Equal(catalogos[i].Alias, catalogosDto[i].Alias);
                Assert.Equal(catalogos[i].Nombre, catalogosDto[i].Nombre);
                Assert.Equal(catalogos[i].VisibleEmpresa, catalogosDto[i].VisibleEmpresa);
                Assert.Equal(catalogos[i].RequiereFirma, catalogosDto[i].RequiereFirma);
                Assert.Equal(catalogos[i].AccionControlador, catalogosDto[i].AccionControlador);
            }
        }

        [Fact]
        public async Task ObtenerCatalogosPorInstitucion_DevuelveError()
        {           

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerCatalogosPorIdInstitucionQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());            

            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.ObtenerCatalogosPorInstitucion(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);

            Assert.Equal("404", objectResult.Error.ErrorCode);
        }

        [Fact]
        public async Task ObtenerCatalogosPorInstitucion_DevuelveBadRequest()
        {
            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdInstitucion", "Required");

            var result = await controller.ObtenerCatalogosPorInstitucion(0);
            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ObtenerCatalogosPorInstitucion_DevuelveBadRequestPorModeloInvalido()
        {
            var controller = new CatalogosController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("IdInstitucion", "Required");

            var result = await controller.ObtenerCatalogosPorInstitucion(0);

            Assert.IsType<ODataErrorResult>(result);
        }

        [Fact]
        public async Task ObtenerCatalogoPorId_Ok()
        {
            //Arrange
            var catalogo = new Catalogo()
            {
                Id = 1,
                Nombre = "Catalogo 1",
            };

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCatalogoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(catalogo);

            var controller = new CatalogosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);
            var queryResult = Assert.IsType<OkObjectResult>(result);
            var querydto = Assert.IsType<CatalogoDto>(queryResult.Value);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CatalogoDto>(queryResult.Value);
            Assert.Equal(1, querydto.Id);
            Assert.Equal("Catalogo 1", querydto.Nombre);
        }
        [Fact]
        public async Task ObtenerCasaPorId_Error()
        {
            var error = ErroresCatalogo.NoEncontrado;

            //Act
            var mockRepo = new Mock<IUnitOfWork>();
            var mockSender = new Mock<ISender>();
            mockSender.Setup(s => s.Send(It.IsAny<ObtenerCatalogoPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(error);

            var controller = new CatalogosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            var result = await controller.Get(1);

            //Assert
            var errorRes = Assert.IsType<ODataErrorResult>(result);
            Assert.Equal("404", errorRes.Error.ErrorCode);

        }
        [Fact]
        public async Task ObtenerCatalogoPorId_InvalidModel()
        {
            var mockSender = new Mock<ISender>();
            var controller = new CatalogosController(mockSender.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);
            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
