using Moq;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.EstadosExcepcionesMorosidad.Queries.ObtenerEstadosExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class EstadosExcepcionesMorosidadTest
    {
        private Mock<IUnitOfWork> mockRepo;

        public EstadosExcepcionesMorosidadTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerEstadosExcepcionesMorosidad_Ok()
        {
            //Act
            var estados = new List<EstadoExcepcionMorosidad>()
            {
                new EstadoExcepcionMorosidad()
                {
                    Id =1,
                    Nombre = "Tipo 1"
                },
                new EstadoExcepcionMorosidad()
                {
                    Id = 2,
                    Nombre = "Tipo 2"
                }
            };

            ObtenerEstadosExcepcionesMorosidadQuery obtenerEstadosExcepcionesMorosidadQuery = new ObtenerEstadosExcepcionesMorosidadQuery();
            mockRepo.Setup(repo => repo.EstadosExcepcionesMorosidadRepository.ObtenerEstadosExcepcionesMorosidad()).ReturnsAsync(estados);
            var handler = new ObtenerEstadosExcepcionesMorosidadQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerEstadosExcepcionesMorosidadQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<EstadoExcepcionMorosidad>>(result.Value);
        }

        [Fact]
        public async Task ObtenerEstadoExcepcionMorosidadPorId_Ok()
        {
            //Arange
            var estado = new EstadoExcepcionMorosidad()
            {
                Id = 1,
                Nombre = "Tipo 1",
                ExcepcionesMorosidad = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad() { Id = 1 } }
            };

            //Act
            ObtenerEstadosExcepcionesMorosidadPorIdQuery obtenerEstadosExcepcionesMorosidadPorIdQuery = new ObtenerEstadosExcepcionesMorosidadPorIdQuery();
            mockRepo.Setup(repo => repo.EstadosExcepcionesMorosidadRepository.ObtenerEstadoExcepcionMorosidadPorId(1)).ReturnsAsync(estado);
            var handler = new ObtenerEstadosExcepcionesMorosidadPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerEstadosExcepcionesMorosidadPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }
    }
}
