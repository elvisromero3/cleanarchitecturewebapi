using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Queries.ObtenerNoticiasVucePorId;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.CrearNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EditarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarNoticiasVuce;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.ImportarDatos;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EliminarMasivoNoticiasVuce;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class NoticiasVuceTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        public NoticiasVuceTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerNoticiasVuce_Ok()
        {
            //Act
            var noticiasVuce = new List<Dominio.Entidades.NoticiasVuce>()
            {
                new Dominio.Entidades.NoticiasVuce()
                {
                    Id = 1,
                    Titulo = "Noticia Vuce Titulo 1",
                    Texto = "Noticia Vuce TExto",
                    Enlace = "Enlace Vuce",
                    TextoIngles = "News Vuce Text",
                    TituloIngles = "News Vue Title"
                }
            };

            ObtenerNoticiasVuceQuery obtenerNoticiasVuceQuery = new ObtenerNoticiasVuceQuery();
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(noticiasVuce);
            var handler = new ObtenerNoticiasVuceQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerNoticiasVuceQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.NoticiasVuce>>(result.Value);
        }

        [Fact]
        public async Task ObtenerNoticiasVucePorId_Ok()
        {
            //Arange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            //Act
            ObtenerNoticiasVucePorIdQuery obtenerNoticiasVucePorIdQuery = new ObtenerNoticiasVucePorIdQuery();
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            var handler = new ObtenerNoticiasVucePorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerNoticiasVucePorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_Ok()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Noticia Vuce Titulo 1" };

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(noticiasVuce);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_Error()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Noticia Vuce Titulo 1" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_NoExiste()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Casillero" };
            var errorIsError = ErroresNoticiasVuce.NoEncontrada;

            //Act

            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.NoEncontrada", result.FirstError.Code);
            Assert.Equal("NoticiasVuce no encontrada", result.FirstError.Description);
        }            

        [Fact]
        public async Task EliminarNoticiasVuce_Ok()
        {
            //Arrange
            var idNoticiasVuce = 1;
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            //Act
            EliminarNoticiasVuceCommand command = new EliminarNoticiasVuceCommand();
            command.Id = idNoticiasVuce;
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(idNoticiasVuce)).ReturnsAsync(noticiasVuce); 
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(idNoticiasVuce)).ReturnsAsync(Result.Deleted); 
            var handler = new EliminarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarNoticiasVuce_ObtenerNoticiasVucePorIdDevuelveError()
        {
            //Arrange
            var idNoticiasVuce = 1;

            //Act
            EliminarNoticiasVuceCommand command = new EliminarNoticiasVuceCommand();
            command.Id = idNoticiasVuce;
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(idNoticiasVuce)).ReturnsAsync(Error.Failure());
            var handler = new EliminarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarNoticiasVuce_Error()
        {
            //Arrange
            var idNoticiasVuce = 1;
            var errorIsError = ErroresNoticiasVuce.NoEncontrada;
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            //Act
            EliminarNoticiasVuceCommand command = new EliminarNoticiasVuceCommand();
            command.Id = idNoticiasVuce;
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(idNoticiasVuce)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(idNoticiasVuce)).ReturnsAsync(errorIsError);
            var handler = new EliminarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.NoEncontrada", result.FirstError.Code);
            Assert.Equal("NoticiasVuce no encontrada", result.FirstError.Description);
        }         

        [Fact]
        public async Task CrearNoticiasVuce_Ok()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "https://futbolcentroamerica.com/tema/seleccion-costa-rica",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };            
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Noticia Vuce Titulo 1", result.Value.Titulo);            
        }

        [Fact]
        public async Task CrearNoticiasVuce_Error()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };


            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(errorIsError);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.NoticiasVuce>() { new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "https://futbolcentroamerica.com/tema/seleccion-costa-rica",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title" }
            };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(It.IsAny<Dominio.Entidades.NoticiasVuce>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerpaisesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.NoticiasVuce>() { new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title" }
            };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarPaisDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.NoticiasVuce>() { new Dominio.Entidades.NoticiasVuce()
            {
                Id = -1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title" }
            };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(-1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearPaisDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.NoticiasVuce>() { new Dominio.Entidades.NoticiasVuce()
            {
                Id = -1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title" }
            };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(It.IsAny<Dominio.Entidades.NoticiasVuce>())).ReturnsAsync(Error.Failure());

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
            var datos = new List<Dominio.Entidades.NoticiasVuce>()
            {
                new Dominio.Entidades.NoticiasVuce() { Titulo = "A", TituloIngles="1",Texto="A", TextoIngles="A",Enlace="A" },
                new Dominio.Entidades.NoticiasVuce() { Titulo = "A", TituloIngles="1",Texto="A", TextoIngles="A",Enlace="A" }
            };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(It.IsAny<Dominio.Entidades.NoticiasVuce>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarMasivamenteNoticiasVuce_DevuelveOk()
        {
            // Arrange
            var idsNoticias = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarMasivoNoticiasVuceCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarMasivoNoticiasVuceCommand { IdsNoticias = idsNoticias }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarMasivamenteNoticiasVuce_DevuelveError()
        {
            // Arrange
            var idsNoticias = new List<int> { 1, 2, 3 };
            var erroror = ErroresVariedad.NoEncontrado;

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.EliminarNoticiasVuce(3)).ReturnsAsync(erroror);

            var handler = new EliminarMasivoNoticiasVuceCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarMasivoNoticiasVuceCommand { IdsNoticias = idsNoticias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarNoticiasVuceCrear_DevuelveError()
        {

            // Arrange

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "https://futbolcentroamerica.com/tema/seleccion-costa-rica",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(noticiasVuce.Titulo, noticiasVuce.Texto, noticiasVuce.Enlace, noticiasVuce.TituloIngles, noticiasVuce.TextoIngles)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearNoticiasVuceCommand { NoticiasVuce = noticiasVuce }, default);


            // Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("NoticiasVuce contiene datos duplicados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarNoticiasVuceEditar_DevuelveExiste()
        {

            // Arrange            

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            IEnumerable<string> listaCambios = new List<string> { "Titulo" };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(noticiasVuce.Titulo, noticiasVuce.Texto, noticiasVuce.Enlace, noticiasVuce.TituloIngles, noticiasVuce.TextoIngles)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(noticiasVuce, noticiasVuce.Id, listaCambios)).ReturnsAsync(noticiasVuce);

            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("NoticiasVuce contiene datos duplicados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarNoticiasVuceEditar_DevuelveError()
        {

            // Arrange            

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            IEnumerable<string> listaCambios = new List<string> { "Titulo", "Texto", "Enlace", "TituloIngles", "TextoIngles", };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(noticiasVuce.Titulo, noticiasVuce.Texto, noticiasVuce.Enlace, noticiasVuce.TituloIngles, noticiasVuce.TextoIngles)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(noticiasVuce, noticiasVuce.Id, listaCambios)).ReturnsAsync(noticiasVuce);

            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }
        
        [Fact]
        public async Task ImportarDatos_ValidarNoticiasVuceDevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.NoticiasVuce>() { new Dominio.Entidades.NoticiasVuce()
            {
                Id = -1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title" }
            };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(datos[0].Titulo, datos[0].Texto, datos[0].Enlace, datos[0].TituloIngles, datos[0].TextoIngles)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.Equal("NoticiasVuce.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("NoticiasVuce contiene datos duplicados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_DevuelveValidacionNoticiasTituloError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInvalido;

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(error);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearNoticiasVuce_DevuelveValidacionNoticiasTituloInglesError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",                
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"

            };

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(error);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearNoticiasVuce_DevuelveValidacionNoticiasLinkError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            var error = ErroresNoticiasVuce.NoticiasLinkExcedeLimite;

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(error);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }


        [Fact]
        public async Task CrearNoticiasVuce_ErrorTitulo()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.TituloInvalido", result.FirstError.Code);
            Assert.Equal("El título de noticias VUCE es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_ErrorTituloIngles()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "título",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "abc",
                TituloIngles = ""

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.TituloInglesInvalido", result.FirstError.Code);
            Assert.Equal("El título en ingles de noticias VUCE es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_ErrorEnlace()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "título",
                Texto = "abc",
                Enlace = "Enlace Vuce Enlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace Vuce",
                TextoIngles = "News Vuce Title",
                TituloIngles = "News Vuce Title"

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ValidacionNoticiasLink", result.FirstError.Code);
            Assert.Equal("El enlace debe tener máximo 250 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_ErrorTexto()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "título",
                Texto = "",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Title",
                TituloIngles = "News Vuce Title"

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.TextoInvalido", result.FirstError.Code);
            Assert.Equal("El texto de noticias VUCE es un campo requerido y su tamaño debe ser mayor a 0", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearNoticiasVuce_ErrorTextoIngles()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "título",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "",
                TituloIngles = "News Vue Title"

            };

            //Act
            CrearNoticiasVuceCommand command = new CrearNoticiasVuceCommand() { NoticiasVuce = noticiasVuce };
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.CrearNoticiasVuce(noticiasVuce)).ReturnsAsync(noticiasVuce);

            var handler = new CrearNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("NoticiasVuce.TextoInglesInvalido", result.FirstError.Code);
            Assert.Equal("El texto en inglés de noticias VUCE es un campo requerido y su tamaño debe ser mayor a 0", result.FirstError.Description);
        }


        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasTituloError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Titulo" };

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInvalido;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        
        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasTituloInglesError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",                
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            var listaCambios = new List<string> { "TituloIngles" };

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasTextoError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Título",
                Texto = "",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Texto" };

            var error = ErroresNoticiasVuce.NoticiasVuceTextoInvalido;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasTextoInglesError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Título",
                Texto = "texto",
                Enlace = "Enlace Vuce",
                TextoIngles = "",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "TextoIngles" };

            var error = ErroresNoticiasVuce.NoticiasVuceTextoInglesInvalido;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasEnlaceError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Título",
                Texto = "texto",
                Enlace = "Enlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace VuceEnlace Vuce",
                TextoIngles = "texto",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Enlace" };

            var error = ErroresNoticiasVuce.NoticiasLinkExcedeLimite;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarNoticiasVuce_DevuelveValidacionNoticiasLinkError()
        {
            //Arrange
            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var listaCambios = new List<string> { "Titulo" };

            var error = ErroresNoticiasVuce.NoticiasLinkExcedeLimite;

            //Act
            EditarNoticiasVuceCommand command = new EditarNoticiasVuceCommand(noticiasVuce, noticiasVuce.Id, listaCambios);

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVucePorId(1)).ReturnsAsync(noticiasVuce);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarNoticiasVuceCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNoticiasTituloError()
        {
            // Arrange
            var modo = 2;

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };


            var datos = new List<Dominio.Entidades.NoticiasVuce>() { noticiasVuce };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(datos[0].Titulo, datos[0].Texto, datos[0].Enlace, datos[0].TituloIngles, datos[0].TextoIngles)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInvalido;

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNoticiasTituloInglesError()
        {
            // Arrange
            var modo = 2;

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "Enlace Vuce",
                TextoIngles = "News Vuce Text",
                TituloIngles = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            var datos = new List<Dominio.Entidades.NoticiasVuce>() { noticiasVuce };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(datos[0].Titulo, datos[0].Texto, datos[0].Enlace, datos[0].TituloIngles, datos[0].TextoIngles)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            var error = ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNoticiasLinkErrorRegex()
        {
            // Arrange
            var modo = 2;

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "111",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var datos = new List<Dominio.Entidades.NoticiasVuce>() { noticiasVuce };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(datos[0].Titulo, datos[0].Texto, datos[0].Enlace, datos[0].TituloIngles, datos[0].TextoIngles)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            var error = ErroresNoticiasVuce.NoticiasLinkInValido;

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNoticiasLinkErrorLongitud()
        {
            // Arrange
            var modo = 2;

            var noticiasVuce = new Dominio.Entidades.NoticiasVuce()
            {
                Id = 1,
                Titulo = "Noticia Vuce Titulo 1",
                Texto = "Noticia Vuce TExto",
                Enlace = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                TextoIngles = "News Vuce Text",
                TituloIngles = "News Vue Title"
            };

            var datos = new List<Dominio.Entidades.NoticiasVuce>() { noticiasVuce };

            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ObtenerNoticiasVuce()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.NoticiasVuceRepository.ValidarNoticiasVuce(datos[0].Titulo, datos[0].Texto, datos[0].Enlace, datos[0].TituloIngles, datos[0].TextoIngles)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            var error = ErroresNoticiasVuce.NoticiasLinkExcedeLimite;

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
    }
}
