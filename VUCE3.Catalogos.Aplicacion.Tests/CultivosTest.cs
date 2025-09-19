using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.CrearCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EditarCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivo;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EliminarCultivos;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Cultivo.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivoPorId;
using VUCE3.Catalogos.Aplicacion.Cultivo.Queries.ObtenerCultivos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CultivosTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task ObtenerCultivos_DevuelveOk()
        {
            // Arrange
            var cultivos = new List<Dominio.Entidades.Cultivo>
            {
                new Dominio.Entidades.Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Dominio.Entidades.Variedad { Id = 2 }
                },
                new Dominio.Entidades.Cultivo
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    Variedad = new Dominio.Entidades.Variedad { Id = 3 }
                },
                new Dominio.Entidades.Cultivo
                {
                    Id = 3,
                   Codigo = "3",
                    Nombre = "Cultivo 3",
                    NombreCientifico = "Cientifico 3",
                    Variedad = new Dominio.Entidades.Variedad { Id = 4 }
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(cultivos);

            var handler = new ObtenerCultivosQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerCultivosQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveOk()
        {
            // Arrange
            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Dominio.Entidades.Variedad { Id = 2 }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);

            var handler = new ObtenerCultivoPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerCultivoPorIdQuery { IdCultivo = 1 }, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(cultivo.Id, result.Value.Id);
        }

        [Fact]
        public async Task ObtenerCultivoPorId_DevuelveError()
        {
            // Arrange
            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(erroror);

            var handler = new ObtenerCultivoPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerCultivoPorIdQuery { IdCultivo = 1 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearCultivo_ErrorCodigo()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 25 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCultivo_ErrorIdVariedad()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "ABC",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 0,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.IdVariedadInvalido", result.FirstError.Code);
            Assert.Equal("El identificador de variedad es un campo requerido y su valor debe ser mayor a 0", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCultivo_ErrorNombreCientifico()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "ABC",
                Nombre = "Cultivo 1",
                NombreCientifico = "",
                IdVariedad = 1,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.NombreCientificoInvalido", result.FirstError.Code);
            Assert.Equal("El nombre cientifico es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCultivo_ErrorNombre()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "ABC",
                Nombre = "",
                NombreCientifico = "Nombre c",
                IdVariedad = 1,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveErrorVariedad()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad= 1,
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveErrorIdVariedad()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad", "Nombre", "NombreCientifico" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveError()
        {
            // Arrange
            var erroror = ErroresCultivo.NoEncontrado;

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            var command = new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(command.Cultivo, command.IdCultivo, command.ListaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorVariedad()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad =1,
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorListaCambios()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            IEnumerable<string> listaCambios = new List<string> { };
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorIdVariedad()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveErrorIdVariedadListaCambios()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad =1,
                Variedad = variedad
            };

            var erroror = ErroresVariedad.NoEncontrado;

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_CodigoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "Codigo" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EditarCultivo_NombreCientificoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "123",
                Nombre = "Cultivo 1",
                NombreCientifico = "",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "NombreCientifico" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EditarCultivo_NombreError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "123",
                Nombre = "",
                NombreCientifico = "Cultivo 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "Nombre" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EditarCultivo_IdVariedadError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "123",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cultivo 1",
                IdVariedad = 0
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCultivo_DevuelveOk()
        {
            // Arrange
            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Dominio.Entidades.Variedad { Id = 2 }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCultivoCommand { IdCultivo = 1 }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCultivo_ObtenerCultivoPorIdDevuelveError()
        {
            // Arrange

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(Error.Failure());

            var handler = new EliminarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCultivoCommand { IdCultivo = 1 }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCultivo_DevuelveError()
        {
            // Arrange
            var idCultivo = 1;
            var erroror = ErroresCultivo.NoEncontrado;
            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Dominio.Entidades.Variedad { Id = 2 }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(idCultivo)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(idCultivo)).ReturnsAsync(erroror);

            var handler = new EliminarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCultivoCommand { IdCultivo = idCultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = "A", NombreCientifico = "A", Variedad = new Dominio.Entidades.Variedad { Codigo = "1" } } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(datos[0].Variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(It.IsAny<Dominio.Entidades.Cultivo>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_NombreLongitudNombreMayor100DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = new string('A', 101), Variedad = new Dominio.Entidades.Variedad { Codigo = "1" } } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = new string('A', 101),
                    NombreCientifico = "A",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(datos[0].Variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(It.IsAny<Dominio.Entidades.Cultivo>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_NombreCientificoLongitudNombreCientificoMayor100DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = "A", NombreCientifico = new string('A', 101), Variedad = new Dominio.Entidades.Variedad { Codigo = "1" } } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = new string('A', 101),
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(datos[0].Variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(It.IsAny<Dominio.Entidades.Cultivo>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerCultivoesDevuelveError()
        {
            // Arrange
            var modo = 2;

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    CodigoVariedad = "1",
                    NombreCientifico = "A"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarCultivoDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = "A", NombreCientifico="A" } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico="A"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearCultivoDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = "A", NombreCientifico = "A", Variedad = new Dominio.Entidades.Variedad { Codigo = "1" } } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(datos[0].Variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(It.IsAny<Dominio.Entidades.Cultivo>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerVariedadPorCodigo_NoExisteDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { 
                new Dominio.Entidades.Cultivo() { 
                    Codigo = "1", 
                    Nombre = "A", 
                    NombreCientifico = "A",
                    Variedad = new Dominio.Entidades.Variedad { Codigo = "1" }
                } 
            };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>()
            {
                new Dominio.Entidades.Cultivo() { Nombre = "A", Codigo="1",NombreCientifico="A" },
                new Dominio.Entidades.Cultivo() { Nombre = "A", Codigo="1",NombreCientifico="A" }
            };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A"
                },
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(It.IsAny<Dominio.Entidades.Cultivo>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCultivos_DevuelveOk()
        {
            // Arrange
            var idsCultivos = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarCultivosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCultivosCommand { IdsCultivos = idsCultivos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCultivos_DevuelveError()
        {
            // Arrange
            var idsCultivos = new List<int> { 1, 2, 3 };
            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CultivosRepository.EliminarCultivo(3)).ReturnsAsync(erroror);

            var handler = new EliminarCultivosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCultivosCommand { IdsCultivos = idsCultivos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarCultivosCrear_DevuelveError()
        {            
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad=1,
                Variedad = variedad
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(cultivo.Id,cultivo.Codigo,cultivo.Nombre,cultivo.NombreCientifico)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(cultivo);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un cultivo con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCultivosEditar_DevuelveExiste()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad", "Codigo" };
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(cultivo.Id, cultivo.Codigo, cultivo.Nombre, cultivo.NombreCientifico)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Cultivo.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un cultivo con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCultivosEditar_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad","Codigo" };
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(cultivo.Id, cultivo.Codigo, cultivo.Nombre, cultivo.NombreCientifico)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(cultivo);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarCultivoDevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Cultivo>() { new Dominio.Entidades.Cultivo() { Codigo = "1", Nombre = "A", NombreCientifico = "A", Variedad = new Dominio.Entidades.Variedad { Codigo = "1" } } };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "A",
                    NombreCientifico = "A",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorCodigo("1")).ReturnsAsync(datos[0].Variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.Equal("Cultivo.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un cultivo con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveValidacionCodigoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "11111111111111111111111111",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresCultivo.CultivoCodigoInvalido;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveValidacionNombreError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresCultivo.CultivoNombreInvalido;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveValidacionNombreCientificoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                IdVariedad = 1,
                Variedad = variedad
            };

            var erroror = ErroresCultivo.CultivoNombreCientificoInvalido;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearCultivo_DevuelveValidacionIdVariedadError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cultivo 111",
                IdVariedad = 1234567891,
                Variedad = variedad
            };

            var erroror = ErroresCultivo.IdVarierdadExcedeLimite;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.CrearCultivo(cultivo)).ReturnsAsync(erroror);

            var handler = new CrearCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearCultivoCommand { Cultivo = cultivo }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveValidacionCodigoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "11111111111111111111111111",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            var erroror = ErroresCultivo.CultivoCodigoInvalido;

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveValidacionNombreError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            var erroror = ErroresCultivo.CultivoNombreInvalido;

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveValidacionNombreCientificoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                IdVariedad = 1
            };

            var erroror = ErroresCultivo.CultivoNombreCientificoInvalido;

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EditarCultivo_DevuelveValidacionIdVariedadError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1 };

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1234567891
            };

            IEnumerable<string> listaCambios = new List<string> { "IdVariedad" };

            var erroror = ErroresCultivo.IdVarierdadExcedeLimite;

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivoPorId(1)).ReturnsAsync(cultivo);
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.CultivosRepository.EditarCultivo(cultivo, cultivo.Id, listaCambios)).ReturnsAsync(erroror);            

            var handler = new EditarCultivoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarCultivoCommand { IdCultivo = cultivo.Id, Cultivo = cultivo, ListaCambios = listaCambios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoError()
        {
            // Arrange

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "11111111111111111111111111",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { cultivo };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "11111111111111111111111111",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(datos[0].Id, datos[0].Codigo, datos[0].Nombre, datos[0].NombreCientifico)).ReturnsAsync(true);

            var error = ErroresCultivo.CultivoCodigoInvalido;

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreError()
        {
            // Arrange

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NombreCientifico = "Cientifico 1",
                IdVariedad = 1
            };

            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { cultivo };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                    NombreCientifico = "Cientifico 1",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(datos[0].Id, datos[0].Codigo, datos[0].Nombre, datos[0].NombreCientifico)).ReturnsAsync(true);

            var error = ErroresCultivo.CultivoNombreInvalido;

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        
        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreCientificoError()
        {
            // Arrange

            var cultivo = new Dominio.Entidades.Cultivo
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                IdVariedad = 1
            };            

            var modo = 2;
            var datos = new List<Dominio.Entidades.Cultivo>() { cultivo };

            var datosDto = new List<ImportarCultivoCommandDto>() {
                new ImportarCultivoCommandDto() {
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cultivo 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                    CodigoVariedad = "1"
                }
            };

            mockRepo.Setup(repo => repo.CultivosRepository.ObtenerCultivos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CultivosRepository.ValidarCultivo(datos[0].Id, datos[0].Codigo, datos[0].Nombre, datos[0].NombreCientifico)).ReturnsAsync(true);

            var error = ErroresCultivo.CultivoNombreCientificoInvalido;

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }        


    }
}
