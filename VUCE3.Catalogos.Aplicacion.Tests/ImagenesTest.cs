using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenesPorId;
using VUCE3.Catalogos.Aplicacion.Imagenes.Queries.ObtenerImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.CrearImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EditarImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarImagenes;
using VUCE3.Catalogos.Aplicacion.Imagenes.Commands.EliminarMasivoImagenes;
using ErrorOr;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ImagenesTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        public ImagenesTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerImagenes_Ok()
        {
            //Act
            var imagenes = new List<Dominio.Entidades.Imagenes>()
            {
                new Dominio.Entidades.Imagenes()
                {
                    Id =1,
                    Imagen = dataImageMock64()
                }
            };
            

            ObtenerImagenesQuery obtenerImagenesQuery = new ObtenerImagenesQuery();
            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenes()).ReturnsAsync(imagenes);
            var handler = new ObtenerImagenesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerImagenesQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Imagenes>>(result.Value);
        }

        [Fact]
        public async Task ObtenerImagenPorId_Ok()
        {
            //Arange
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
                
            };

            //Act
            ObtenerImagenesPorIdQuery obtenerImagenPorIdQuery = new ObtenerImagenesPorIdQuery();
            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(1)).ReturnsAsync(imagen);
            var handler = new ObtenerImagenesPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerImagenPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarImagen_Ok()
        {
            //Arrange

            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),

            };


            var listaCambios = new List<string> { dataImageMock64() };

            //Act
            EditarImagenesCommand command = new EditarImagenesCommand(imagen, imagen.Id, listaCambios);

            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(1)).ReturnsAsync(imagen);
            mockRepo.Setup(repo => repo.ImagenesRepository.ActualizarImagen(command.Imagenes, command.IdImagenes, command.ListaCambios)).ReturnsAsync(imagen);
            var handler = new EditarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarImagen_Error()
        {
            //Arrange
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };

            var listaCambios = new List<string> { dataImageMock64() };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarImagenesCommand command = new EditarImagenesCommand(imagen, imagen.Id, listaCambios);

            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(1)).ReturnsAsync(imagen);
            mockRepo.Setup(repo => repo.ImagenesRepository.ActualizarImagen(command.Imagenes, command.IdImagenes, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarImagen_NoExiste()
        {
            //Arrange
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };

            var listaCambios = new List<string> { dataImageMock64() };
            var errorIsError = ErroresImagenes.NoEncontrada;

            //Act
            EditarImagenesCommand command = new EditarImagenesCommand(imagen, imagen.Id, listaCambios);

            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ImagenesRepository.ActualizarImagen(command.Imagenes, command.IdImagenes, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Imagen.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Imagen no encontrada", result.FirstError.Description);
        }            

        [Fact]
        public async Task EliminarImagen_Ok()
        {
            //Arrange
            var idImagen = 1;
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };

            //Act
            EliminarImagenesCommand command = new EliminarImagenesCommand();
            command.Id = idImagen;
            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(idImagen)).ReturnsAsync(imagen);
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(idImagen)).ReturnsAsync(Result.Deleted); 
            var handler = new EliminarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarImagen_ObtenerImagenPorIdDevuelveError()
        {

            //Act
            EliminarImagenesCommand command = new EliminarImagenesCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(1)).ReturnsAsync(Error.Failure());
            var handler = new EliminarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarImageb_NoExiste()
        {
            //Arrange
            var idImagen = 1;
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };
            var errorIsError = ErroresImagenes.NoEncontrada;

            //Act
            EliminarImagenesCommand command = new EliminarImagenesCommand();
            command.Id = idImagen;
            mockRepo.Setup(repo => repo.ImagenesRepository.ObtenerImagenPorId(idImagen)).ReturnsAsync(imagen);
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(idImagen)).ReturnsAsync(errorIsError);
            var handler = new EliminarImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Imagen.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Imagen no encontrada", result.FirstError.Description);
        }         

        [Fact]
        public async Task CrearImagen_Ok()
        {
            //Arrange
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };

            //Act
            CrearImagenesCommand command = new CrearImagenesCommand() { Imagenes = imagen };            
            mockRepo.Setup(repo => repo.ImagenesRepository.CrearImagen(imagen)).ReturnsAsync(imagen);

            var handler = new CrearImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal(dataImageMock64(), result.Value.Imagen);            
        }

        [Fact]
        public async Task CrearImagen_Error()
        {
            //Arrange
            var imagen = new Dominio.Entidades.Imagenes()
            {
                Id = 1,
                Imagen = dataImageMock64(),
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearImagenesCommand command = new CrearImagenesCommand() { Imagenes = imagen };            
            mockRepo.Setup(repo => repo.ImagenesRepository.CrearImagen(imagen)).ReturnsAsync(errorIsError);

            var handler = new CrearImagenesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        private static string dataImageMock64()
        {
            return "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/7QE8UGhvdG9zaG9wIDMuMAA4QklNBAQAAAAAASAcAlAACkNISUFSSV9WRlgcAlUAC0NvbnRyaWJ1dG9yHAJ4AEMzRCBSZW5kZXJpbmcgY29vbCBlbW9qaSB3aXRoIHN1bmdsYXNzIGlzb2xhdGVkIG9uIHdoaXRlIGJhY2tncm91bmQuHAJlAAZGcmFuY2UcAmQAA0ZSQRwCNwAIMjAxODA1MDQcAm4AGEdldHR5IEltYWdlcy9pU3RvY2twaG90bxwCaQBCM0QgUmVuZGVyaW5nIGNvb2wgZW1vamkgd2l0aCBzdW5nbGFzcyBpc29sYXRlZCBvbiB3aGl0ZSBiYWNrZ3JvdW5kHAIoABJOb3QgUmVsZWFzZWQgKE5SKSAcAgUACTk1NTAzNjgyNhwCcwALaVN0b2NrcGhvdG//4QU0aHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwAJPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KCQk8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczpwaG90b3Nob3A9Imh0dHA6Ly9ucy5hZG9iZS5jb20vcGhvdG9zaG9wLzEuMC8iIHhtbG5zOklwdGM0eG1wQ29yZT0iaHR0cDovL2lwdGMub3JnL3N0ZC9JcHRjNHhtcENvcmUvMS4wL3htbG5zLyIgeG1sbnM6R2V0dHlJbWFnZXNHSUZUPSJodHRwOi8veG1wLmdldHR5aW1hZ2VzLmNvbS9naWZ0LzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGx1cz0iaHR0cDovL25zLnVzZXBsdXMub3JnL2xkZi94bXAvMS4wLyIgeG1sbnM6aXB0Y0V4dD0iaHR0cDovL2lwdGMub3JnL3N0ZC9JcHRjNHhtcEV4dC8yMDA4LTAyLTI5LyIgcGhvdG9zaG9wOlNvdXJjZT0iaVN0b2NrcGhvdG8iIHBob3Rvc2hvcDpBdXRob3JzUG9zaXRpb249IkNvbnRyaWJ1dG9yIiBwaG90b3Nob3A6Q29weXJpZ2h0RmxhZz0idHJ1ZSIgcGhvdG9zaG9wOkNvdW50cnk9IkZyYW5jZSIgSXB0YzR4bXBDb3JlOkNvdW50cnlDb2RlPSJGUkEiIHBob3Rvc2hvcDpEYXRlQ3JlYXRlZD0iMjAxOC0wNS0wNFQwNzowMDowMCswMDowMCIgcGhvdG9zaG9wOkNyZWRpdD0iR2V0dHkgSW1hZ2VzL2lTdG9ja3Bob3RvIiBHZXR0eUltYWdlc0dJRlQ6RGxyZWY9Im42dFg0eDFwd1JtSFd0TTBkMDg5Smc9PSIgcGhvdG9zaG9wOkhlYWRsaW5lPSIzRCBSZW5kZXJpbmcgY29vbCBlbW9qaSB3aXRoIHN1bmdsYXNzIGlzb2xhdGVkIG9uIHdoaXRlIGJhY2tncm91bmQiIHBob3Rvc2hvcDpVUkw9Imh0dHA6Ly93d3cuaXN0b2NrLmNvbSIgR2V0dHlJbWFnZXNHSUZUOkltYWdlUmFuaz0iMyIgcGhvdG9zaG9wOkluc3RydWN0aW9ucz0iTm90IFJlbGVhc2VkIChOUikgIiBwbHVzOkxpY3NlbnNvclVSTD0iaHR0cDovL3d3dy5nZXR0eWltYWdlcy5jb20iIEdldHR5SW1hZ2VzR0lGVDpBc3NldElEPSI5NTUwMzY4MjYiIGRjOnRpdGxlPSI5NTUwMzY4MjYiID4KPGRjOmNyZWF0b3I+PHJkZjpTZXE+PHJkZjpsaT5DSElBUklfVkZYPC9yZGY6bGk+PC9yZGY6U2VxPjwvZGM6Y3JlYXRvcj48ZGM6ZGVzY3JpcHRpb24+PHJkZjpBbHQ+PHJkZjpsaSB4bWw6bGFuZz0ieC1kZWZhdWx0Ij4zRCBSZW5kZXJpbmcgY29vbCBlbW9qaSB3aXRoIHN1bmdsYXNzIGlzb2xhdGVkIG9uIHdoaXRlIGJhY2tncm91bmQuPC9yZGY6bGk+PC9yZGY6QWx0PjwvZGM6ZGVzY3JpcHRpb24+CgkJPC9yZGY6RGVzY3JpcHRpb24+Cgk8L3JkZjpSREY+Cv/bAEMABgQEBQQEBgUFBQYGBgcJDgkJCAgJEg0NCg4VEhYWFRIUFBcaIRwXGB8ZFBQdJx0fIiMlJSUWHCksKCQrISQlJP/bAEMBBgYGCQgJEQkJESQYFBgkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQkJCQk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";
        }

        [Fact]
        public async Task EliminarImagenes_DevuelveOk()
        {
            // Arrange
            var idsImagenes = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarMasivoImagenesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarMasivoImagenesCommand { IdsImagenes = idsImagenes }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarImagenes_DevuelveError()
        {
            // Arrange
            var idsImagenes = new List<int> { 1, 2, 3 };
            var erroror = ErroresImagenes.NoEncontrada;

            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ImagenesRepository.EliminarImagen(3)).ReturnsAsync(erroror);

            var handler = new EliminarMasivoImagenesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarMasivoImagenesCommand { IdsImagenes = idsImagenes }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
    }
}
