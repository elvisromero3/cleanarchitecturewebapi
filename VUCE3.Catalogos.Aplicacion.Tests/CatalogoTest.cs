using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogoPorId;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogos;
using VUCE3.Catalogos.Aplicacion.Catalogo.Queries.ObtenerCatalogosPorIdInstitucion;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CatalogoTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task GetCatalogos_DevuelveOkAsync()
        {
            // Arrange
            var catalogos = new List<Dominio.Entidades.Catalogo>
            {
                new Dominio.Entidades.Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catalogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 1"
                },
                new Dominio.Entidades.Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catalogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 2"
                }
            };

            mockRepo.Setup(repo => repo.CatalogosRepository.ObtenerCatalogos()).ReturnsAsync(catalogos);

            var handler = new ObtenerCatalogosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(new ObtenerCatalogosQuery(), default);

            Assert.False(result.IsError);
            Assert.Equal(catalogos.Count, result.Value?.Count);
        }

        [Fact]
        public async Task GetCatalogos_DevuelveErrorAsync()
        {
            var erroror = ErroresCatalogo.NoEncontrado;

            // Arrange
            mockRepo.Setup(repo => repo.CatalogosRepository.ObtenerCatalogos()).ReturnsAsync(erroror);

            var handler = new ObtenerCatalogosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(new ObtenerCatalogosQuery(), default);

            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ObtenerCatalogoPorId_Ok()
        {
            //Arange
            var catalogo = new Dominio.Entidades.Catalogo()
            {
                Id = 1,
                Nombre = "Catalogo 1"
            };

            //Act
            ObtenerCatalogoPorIdQuery obtenerCatalogoPorIdQuery = new ObtenerCatalogoPorIdQuery();
            mockRepo.Setup(repo => repo.CatalogosRepository.ObtenerCatalogoPorId(1)).ReturnsAsync(catalogo);
            var handler = new ObtenerCatalogoPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCatalogoPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task GetCatalogosPorInstitucion_DevuelveOkAsync()
        {
            // Arrange
            var catalogos = new List<Dominio.Entidades.Catalogo>
            {
                new Dominio.Entidades.Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catalogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 1"
                },
                new Dominio.Entidades.Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catalogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 2"
                }
            };

            mockRepo.Setup(repo => repo.CatalogosRepository.ObtenerCatalogosPorIdInstitucion(1)).ReturnsAsync(catalogos);

            var handler = new ObtenerCatalogosPorIdInstitucionQueryHandler(mockRepo.Object);
            var result = await handler.Handle(new ObtenerCatalogosPorIdInstitucionQuery { IdInstitucion = 1 }, default);

            Assert.False(result.IsError);
            Assert.Equal(catalogos.Count, result.Value?.Count);
        }

    }
}
