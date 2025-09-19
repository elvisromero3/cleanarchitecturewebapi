using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EditarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedad;
using VUCE3.Catalogos.Aplicacion.Variedad.Commands.EliminarVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedades;
using VUCE3.Catalogos.Aplicacion.Variedad.Queries.ObtenerVariedadPorId;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class VariedadesTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task ObtenerVariedades_DevuelveOk()
        {
            // Arrange
            var variedades = new List<Dominio.Entidades.Variedad>
            {
                new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1", Cultivos = new() },
                new Dominio.Entidades.Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" },
                new Dominio.Entidades.Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(variedades);

            var handler = new ObtenerVariedadesQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerVariedadesQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerVariedadPorId_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);

            var handler = new ObtenerVariedadPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerVariedadPorIdQuery { Id = 1 }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(variedad.Id, result.Value.Id);
            Assert.Equal(variedad.Codigo, result.Value.Codigo);
            Assert.Equal(variedad.Nombre, result.Value.Nombre);
        }

        [Fact]
        public async Task ObtenerVariedadPorId_NoExiste_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };
            var erroror = ErroresVariedad.NoEncontrado;

            ObtenerVariedadPorIdQuery _obtenerVariedadPorIdQuery = new ObtenerVariedadPorIdQuery { Id = 1 };
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);

            var handler = new ObtenerVariedadPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(_obtenerVariedadPorIdQuery, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0,variedad.Codigo,variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(variedad);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearVariedad_ErrorCodigo()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0,variedad.Codigo,variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(variedad);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearVariedad_ErrorNombre()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "ABC", Nombre = "" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0, variedad.Codigo, variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(variedad);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveDatosDuplicados()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0, variedad.Codigo, variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(erroror);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }


        [Fact]
        public async Task EditarVariedad_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };
            var listaCambios = new List<string> { "Codigo" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(It.IsAny<int>())).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.EditarVariedad(variedad, variedad.Id, listaCambios)).ReturnsAsync(variedad);

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EditarVariedad_ObtenerVariedadPorIdDevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };
            var listaCambios = new List<string> { "Codigo" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EditarVariedad_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1 Editado" };
            var listaCambios = new List<string> { "Nombre" };
            var erroror = ErroresVariedad.NoEncontrado;

            EditarVariedadCommand _editarVariedadCommand = new EditarVariedadCommand(variedad, variedad.Id, listaCambios);

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(It.IsAny<int>())).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.EditarVariedad(variedad, variedad.Id, listaCambios)).ReturnsAsync(erroror);

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(_editarVariedadCommand, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EliminarVariedad_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = 1 }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.IsType<Dominio.Entidades.Variedad>(result.Value);
        }

        [Fact]
        public async Task EliminarVariedad_RelacionCultivo_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.ExisteRelacionConCultivo(1)).ReturnsAsync(Error.Unexpected());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = 1 }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.FirstError.Code);
            Assert.Equal("An unexpected error has occurred.", result.FirstError.Description);
        }
        [Fact]
        public async Task EliminarVariedad_RelacionCultivo_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.ExisteRelacionConCultivo(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = 1 }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.VariedadRelacionCultivo", result.FirstError.Code);
        }


        [Fact]
        public async Task EliminarVariedad_NoExiste_DevuelveError()
        {
            // Arrange
            var idVariedad = 1;
            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(erroror);

            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = idVariedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task EliminarVariedad_Error()
        {
            // Arrange
            var idVariedad = 1;
            var erroror = ErroresVariedad.NoEncontrado;
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(idVariedad)).ReturnsAsync(erroror);

            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = idVariedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
        [Fact]
        public async Task EliminarVariedad_DevuelveError_Referencial_InnerExeption()
        {
            // Arrange
            var idVariedad = 1;
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.Save()).ThrowsAsync(new Exception("Integridad",new Exception("Integridad")));
            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = idVariedad }, default);
            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task EliminarVariedad_DevuelveError_Referencial()
        {
            // Arrange
            var idVariedad = 1;
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };

            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(1)).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.Save()).ThrowsAsync(new Exception("Integridad"));
            var handler = new EliminarVariedadCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new EliminarVariedadCommand { IdVariedad = idVariedad }, default);
            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Variedad>() { new Dominio.Entidades.Variedad() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }        

        [Fact]
        public async Task ImportarDatos_ObtenerVariedadesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Variedad>() { new Dominio.Entidades.Variedad() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarVariedadDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Variedad>() { new Dominio.Entidades.Variedad() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearVariedadDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Variedad>() { new Dominio.Entidades.Variedad() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Variedad>()
            {
                new Dominio.Entidades.Variedad() { Codigo = "123", Nombre="A" },
                new Dominio.Entidades.Variedad() { Codigo = "123", Nombre="A" }
            };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(It.IsAny<Dominio.Entidades.Variedad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarVariedades_DevuelveOk()
        {
            // Arrange
            var idsVariedades = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadesCommand { IdsVariedades = idsVariedades }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminarVariedades_ExisteRelacion_DevuelveError()
        {
            // Arrange
            var idsVariedades = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.VariedadesRepository.ExisteRelacionConCultivo(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadesCommand { IdsVariedades = idsVariedades }, default);

            // Assert
            Assert.True(result.IsError);
         
        }
        [Fact]
        public async Task EliminarVariedades_ExisteRelacion_DevuelveTrue()
        {
            // Arrange
            var idsVariedades = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.VariedadesRepository.ExisteRelacionConCultivo(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarVariedadesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadesCommand { IdsVariedades = idsVariedades }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.VariedadesRelacionCultivo", result.FirstError.Code);
        }
        [Fact]
        public async Task EliminarVariedades_DevuelveError()
        {
            // Arrange
            var idsVariedades = new List<int> { 1, 2, 3 };
            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.VariedadesRepository.EliminarVariedad(3)).ReturnsAsync(erroror);

            var handler = new EliminarVariedadesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarVariedadesCommand { IdsVariedades = idsVariedades }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarVariedadesCrear_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" };            
            
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(variedad.Id,variedad.Codigo,variedad.Nombre)).ReturnsAsync(true);            

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarVariedadesEditar_DevuelveExiste()
        {
            // Arrange

            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1 Editado" };
            var listaCambios = new List<string> { "Nombre" };
            var erroror = ErroresVariedad.NoEncontrado;

            EditarVariedadCommand _editarVariedadCommand = new EditarVariedadCommand(variedad, variedad.Id, listaCambios);

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(It.IsAny<int>())).ReturnsAsync(variedad);
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(variedad.Id,variedad.Codigo,variedad.Nombre)).ReturnsAsync(true);

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ValidarVariedadesEditar_DevuelveError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1 Editado" };
            var listaCambios = new List<string> { "Nombre" };
            var erroror = ErroresVariedad.NoEncontrado;

            EditarVariedadCommand _editarVariedadCommand = new EditarVariedadCommand(variedad, variedad.Id, listaCambios);

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedadPorId(It.IsAny<int>())).ReturnsAsync(variedad);            
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(variedad.Id, variedad.Codigo, variedad.Nombre)).ReturnsAsync(Error.Failure());

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarVariedadesDevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Variedad>() { new Dominio.Entidades.Variedad() { Codigo = "1", Nombre = "A" } };
            
            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.Equal("Variedad.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveValidacionCodigoError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "11111111111111111111111111111", Nombre = "Variedad 1" };

            var error = ErroresVariedad.VariedadCodigoInvalido;

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0, variedad.Codigo, variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(error);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);           

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.CodigoInvalido", result.Errors[0].Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearVariedad_DevuelveValidacionNombreError()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad { 
                    Id = 1, 
                    Codigo = "1", 
                    Nombre = "Variedad 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" 
            };

            var error = ErroresVariedad.VariedadNombreInvalido;

            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(0,variedad.Codigo, variedad.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.VariedadesRepository.CrearVariedad(variedad)).ReturnsAsync(error);

            var handler = new CrearVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearVariedadCommand { Variedad = variedad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.NombreInvalido", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task EditarVariedad_DevuelveValidacionCodigoError()
        {
            // Arrange            

            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "11111111111111111111111111111", Nombre = "Variedad 1" };

            var error = ErroresVariedad.VariedadCodigoInvalido;

            var listaCambios = new List<string> { "Codigo" };

            mockRepo.Setup(repo => repo.VariedadesRepository.EditarVariedad(variedad, variedad.Id, listaCambios)).ReturnsAsync(error);

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.CodigoInvalido", result.Errors[0].Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task EditarVariedad_DevuelveValidacionNombreError()
        {
            // Arrange            

            var variedad = new Dominio.Entidades.Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            var error = ErroresVariedad.VariedadNombreInvalido;

            var listaCambios = new List<string> { "Nombre" };

            mockRepo.Setup(repo => repo.VariedadesRepository.EditarVariedad(variedad, variedad.Id, listaCambios)).ReturnsAsync(error);

            var handler = new EditarVariedadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarVariedadCommand(variedad, variedad.Id, listaCambios), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.NombreInvalido", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoError()
        {
            // Arrange
            var modo = 2;

            var variedad = new Dominio.Entidades.Variedad { Id = 1, Codigo = "11111111111111111111111111111", Nombre = "Variedad 1" };

            var error = ErroresVariedad.VariedadCodigoInvalido;

            var datos = new List<Dominio.Entidades.Variedad>() { variedad };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.CodigoInvalido", result.Errors[0].Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreError()
        {
            // Arrange
            var modo = 2;

            var variedad = new Dominio.Entidades.Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            var error = ErroresVariedad.VariedadNombreInvalido;

            var datos = new List<Dominio.Entidades.Variedad>() { variedad };

            mockRepo.Setup(repo => repo.VariedadesRepository.ObtenerVariedades()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.VariedadesRepository.ValidarVariedad(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Variedad.NombreInvalido", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }
    }
}
