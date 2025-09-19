using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.TiposAccionesExcepcionesMorosidad.Queries.ObtenerTiposAccionesExcepcionesMorosidadPorId;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class TiposAccionesExcepcionesMorosidadTest
    {
        private Mock<IUnitOfWork> mockRepo;

        public TiposAccionesExcepcionesMorosidadTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerTiposAccionesExcepcionesMorosidad_Ok()
        {
            //Act
            var tiposAcciones = new List<TipoAccionExcepcionMorosidad>()
            {
                new TipoAccionExcepcionMorosidad()
                {
                    Id =1,
                    Nombre = "Tipo Acción 1",
                    ExcepcionesMorosidad = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad() { Id = 1 } }
                },
                new TipoAccionExcepcionMorosidad()
                {
                    Id = 2,
                    Nombre = "Tipo Acción 2",
                    ExcepcionesMorosidad = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad() { Id = 2 } }
                }
            };

            ObtenerTiposAccionesExcepcionesMorosidadQuery obtenerTiposAccionesExcepcionesMorosidadQuery = new ObtenerTiposAccionesExcepcionesMorosidadQuery();
            mockRepo.Setup(repo => repo.TiposAccionesExcepcionesMorosidadRepository.ObtenerTiposAccionesExcepcionesMorosidad()).ReturnsAsync(tiposAcciones);
            var handler = new ObtenerTiposAccionesExcepcionesMorosidadQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerTiposAccionesExcepcionesMorosidadQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<TipoAccionExcepcionMorosidad>>(result.Value);
        }

        [Fact]
        public async Task ObtenerRegimenPorId_Ok()
        {
            //Arange
            var tipoAccion = new TipoAccionExcepcionMorosidad()
            {
                Id = 1,
                Nombre = "Tipo acción 1",
                ExcepcionesMorosidad = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad() { Id = 1 } }
            };

            //Act
            ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery obtenerTiposAccionesExcepcionesMorosidadPorIdQuery = new ObtenerTiposAccionesExcepcionesMorosidadPorIdQuery();
            mockRepo.Setup(repo => repo.TiposAccionesExcepcionesMorosidadRepository.ObtenerTipoAccionExcepcionMorosidadPorId(1)).ReturnsAsync(tipoAccion);
            var handler = new ObtenerTiposAccionesExcepcionesMorosidadPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerTiposAccionesExcepcionesMorosidadPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }
    }
}
