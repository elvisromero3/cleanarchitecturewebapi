using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedaPorId;
using VUCE3.Catalogos.Aplicacion.Moneda.Queries.ObtenerMonedas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifaPorId;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifas;
using VUCE3.Catalogos.Aplicacion.Tarifa.Queries.ObtenerTarifasPorId;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class MonedasTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task ObtenerMonedas_DevuelveOk()
        {
            // Arrange
            var monedas = new List<Dominio.Entidades.Moneda>
            {
                new Dominio.Entidades.Moneda
                {
                    Id = 1,
                    Nombre = "Moneda 1",
                    Tarifas = new List<Dominio.Entidades.Tarifa>()
                },
                new Dominio.Entidades.Moneda
                {
                    Id = 2,
                    Nombre = "Moneda 2",
                },
                new Dominio.Entidades.Moneda
                {
                    Id = 3,
                    Nombre = "Moneda 3",
                }
            };

            mockRepo.Setup(repo => repo.MonedasRepository.ObtenerMonedas()).ReturnsAsync(monedas);

            var handler = new ObtenerMonedasQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerMonedasQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerMonedaPorId_DevuelveOk()
        {
            // Arrange
            var moneda = new Dominio.Entidades.Moneda
            {
                Id = 1,
                Nombre = "Moneda 1",
            };

            mockRepo.Setup(repo => repo.MonedasRepository.ObtenerMonedaPorId(1)).ReturnsAsync(moneda);

            var handler = new ObtenerMonedaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerMonedaPorIdQuery { IdMoneda = 1 }, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(moneda.Id, result.Value.Id);
        }

        [Fact]
        public async Task ObtenerMonedaPorId_DevuelveError()
        {
            // Arrange
            var erroror = ErroresMoneda.NoEncontrada;

            mockRepo.Setup(repo => repo.MonedasRepository.ObtenerMonedaPorId(1)).ReturnsAsync(erroror);

            var handler = new ObtenerMonedaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerMonedaPorIdQuery { IdMoneda = 1 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
    }
}
