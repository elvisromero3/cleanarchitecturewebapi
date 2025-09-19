using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Localization;
using Microsoft.OData;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedaPorId;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedas;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Presentacion.Controllers;
using VUCE3.Catalogos.Presentacion.DTO;
using VUCE3.Catalogos.Presentacion.Resources;

namespace VUCE3.Catalogos.Presentacion.Tests
{
    public class MonedasControllerTest
    {
        private Mock<ISender> mockMediator = new Mock<ISender>();
        private readonly Mock<IStringLocalizer<ILocalization>> mockLocalizer = new Mock<IStringLocalizer<ILocalization>>();

        [Fact]
        public async Task ObtenerMonedas_DevuelveOk()
        {
            var monedas = new List<Moneda>
            {
                new Moneda
                {
                    Id = 1,
                    Nombre = "Moneda 1"
                },
                new Moneda
                {
                    Id = 2,
                    Nombre = "Moneda 2"
                },
                new Moneda
                {
                    Id = 3,
                    Nombre = "Moneda 3"
                }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerMonedasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(monedas);

            var controller = new MonedasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get();

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var monedasDto = Assert.IsType<List<MonedaDto>>(objectResult.Value);

            Assert.Equal(monedas.Count, monedasDto.Count);
            for (int i = 0; i < monedas.Count; i++)
            {
                Assert.Equal(monedas[i].Id, monedasDto[i].Id);
                Assert.Equal(monedas[i].Nombre, monedasDto[i].Nombre);
            }
        }

        [Fact]
        public async Task ObtenerMonedas_DevuelveError()
        {

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerMonedasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());
            var controller = new MonedasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerTarifaPorId_DevuelveOk()
        {
            var moneda = new Moneda
            {
                Id = 1,
                Nombre = "Moneda 1",
            };

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerMonedaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(moneda);

            var controller = new MonedasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            //Act
            var result = await controller.Get(1);

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var monedaDto = Assert.IsType<MonedaDto>(objectResult.Value);

            Assert.Equal(moneda.Id, monedaDto.Id);
            Assert.Equal(moneda.Nombre, monedaDto.Nombre);
        }

        [Fact]
        public async Task ObtenerMonedaPorId_DevuelveError()
        {

            mockMediator.Setup(m => m.Send(It.IsAny<ObtenerMonedaPorIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Error.NotFound());

            var controller = new MonedasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);

            var result = await controller.Get(1);

            var objectResult = Assert.IsType<ODataErrorResult>(result);
            var problemDetails = Assert.IsType<ODataError>(objectResult.Error);
            Assert.Equal("404", problemDetails.ErrorCode);
        }

        [Fact]
        public async Task ObtenerMonedaPorId_InvalidModel()
        {

            var controller = new MonedasController(mockMediator.Object, AutoMapperSingleton.Mapper, mockLocalizer.Object);
            controller.ModelState.AddModelError("error", "some error");

            var result = await controller.Get(1);

            Assert.IsType<ODataErrorResult>(result);
        }
    }
}
