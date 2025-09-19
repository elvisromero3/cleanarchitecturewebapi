using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancia;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.EliminarSustancias;
using VUCE3.Catalogos.Aplicacion.Sustancia.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class SustanciasTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        [Fact]
        public async Task ObtenerSustancia_OK()
        {
            // Arrange
            var sustancias = new List<Dominio.Entidades.Sustancia>
            {
                new Dominio.Entidades.Sustancia
                {
                    Id = 1,
                    Nombre = "Sustancia 1",
                    Cas = "123456",
                    ListaCaq = "1A"
                },
                new Dominio.Entidades.Sustancia
                {
                    Id = 2,
                    Nombre = "Sustancia 2",
                    Cas = "654321",
                    ListaCaq = "1B"
                }
            };
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(sustancias);
            var handler = new Sustancia.Queries.ObtenerSustancias.ObtenerSustanciasQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Queries.ObtenerSustancias.ObtenerSustanciasQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerSustanciaPorId_OK()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            var handler = new Sustancia.Queries.ObtenerSustanciaPorId.ObtenerSustanciaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Queries.ObtenerSustanciaPorId.ObtenerSustanciaPorIdQuery { IdSustancia = 1 }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(sustancia.Id, result.Value.Id);
            Assert.Equal(sustancia.Nombre, result.Value.Nombre);
            Assert.Equal(sustancia.Cas, result.Value.Cas);
            Assert.Equal(sustancia.ListaCaq, result.Value.ListaCaq);
        }

        [Fact]
        public async Task ObtenerSustanciaPorId_Error()
        {
            // Arrange
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(ErroresSustancia.NoEncontrado);
            var handler = new Sustancia.Queries.ObtenerSustanciaPorId.ObtenerSustanciaPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Queries.ObtenerSustanciaPorId.ObtenerSustanciaPorIdQuery { IdSustancia = 1 }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSustancia.NoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task CrearSustancia_OK()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
            mockRepo.Setup(x => x.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(sustancia);
            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(sustancia.Id, result.Value.Id);
            Assert.Equal(sustancia.Nombre, result.Value.Nombre);
            Assert.Equal(sustancia.Cas, result.Value.Cas);
            Assert.Equal(sustancia.ListaCaq, result.Value.ListaCaq);
        }

        // DatosDuplicados
        [Fact]
        public async Task CrearSustancia_ErrorDatosDuplicados()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
            mockRepo.Setup(x => x.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(ErroresSustancia.DatosDuplicados);
            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSustancia.DatosDuplicados, result.FirstError);
        }

        [Fact]
        public async Task CrearSustancia_ErrorSustanciaNombreInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "",
                Cas = "25",
                ListaCaq = "ListaCaq"
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(ErroresSustancia.SustanciaNombreInvalido);

            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.NombreInvalido", result.FirstError.Code);
        }

        [Fact]
        public async Task CrearSustancia_ErrorSustanciasCasInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "",
                ListaCaq = "ListaCaq"
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(ErroresSustancia.SustanciasCasInvalido);

            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.CasInvalido", result.FirstError.Code);
        }

        // SustanciaListaCaqInvalido
        [Fact]
        public async Task CrearSustancia_ErrorSustanciaListaCaqInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "5263654",
                ListaCaq = ""
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(ErroresSustancia.SustanciaListaCaqInvalido);

            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.ListaCaqInvalido", result.FirstError.Code);
        }
        [Fact]
        public async Task CrearSustancia_ErrorValidar()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
            mockRepo.Setup(x => x.SustanciasRepository.CrearSustancia(sustancia)).ReturnsAsync(sustancia);
            mockRepo.Setup(x => x.SustanciasRepository.ValidarSustancia(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var handler = new Sustancia.Commands.CrearSustancia.CrearSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.CrearSustancia.CrearSustanciaCommand { Sustancia = sustancia }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task EditarSustancia_OK()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            mockRepo.Setup(x => x.SustanciasRepository.EditarSustancia(sustancia, 1, new List<string> { "Nombre" })).ReturnsAsync(sustancia);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = sustancia, IdSustancia = 1, ListaCambios = new List<string> { "Nombre" } }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(sustancia.Id, result.Value.Item2.Id);
            Assert.Equal(sustancia.Nombre, result.Value.Item2.Nombre);
            Assert.Equal(sustancia.Cas, result.Value.Item2.Cas);
            Assert.Equal(sustancia.ListaCaq, result.Value.Item2.ListaCaq);
        }

        [Fact]
        public async Task EditarSustancia_ErrorSustanciaNoEncontrada()
        {
            // Arrange
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(ErroresSustancia.NoEncontrado);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = new Dominio.Entidades.Sustancia(), IdSustancia = 1, ListaCambios = new List<string> { "Nombre" } }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSustancia.NoEncontrado, result.FirstError);
        }

        // SustanciaNombreInvalido
        [Fact]
        public async Task EditarSustancia_ErrorSustanciaNombreInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "",
                Cas = "25",
                ListaCaq = "ListaCaq"
            };
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            mockRepo.Setup(repo => repo.SustanciasRepository.EditarSustancia(sustancia, 1, new List<string> { "Nombre" })).ReturnsAsync(ErroresSustancia.SustanciaNombreInvalido);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = sustancia, IdSustancia = 1, ListaCambios = new List<string> { "Nombre" } }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.NombreInvalido", result.FirstError.Code);
        }

        // SustanciasCasInvalido
        [Fact]
        public async Task EditarSustancia_ErrorSustanciasCasInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "",
                ListaCaq = "ListaCaq"
            };
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            mockRepo.Setup(repo => repo.SustanciasRepository.EditarSustancia(sustancia, 1, new List<string> { "Cas" })).ReturnsAsync(ErroresSustancia.SustanciasCasInvalido);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = sustancia, IdSustancia = 1, ListaCambios = new List<string> { "Cas" } }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.CasInvalido", result.FirstError.Code);
        }

        // SustanciaListaCaqInvalido
        [Fact]
        public async Task EditarSustancia_ErrorSustanciaListaCaqInvalido()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "5263654",
                ListaCaq = ""
            };
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            mockRepo.Setup(repo => repo.SustanciasRepository.EditarSustancia(sustancia, 1, new List<string> { "ListaCaq" })).ReturnsAsync(ErroresSustancia.SustanciaListaCaqInvalido);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = sustancia, IdSustancia = 1, ListaCambios = new List<string> { "ListaCaq" } }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.ListaCaqInvalido", result.FirstError.Code);
        }
        [Fact]
        public async Task EditarSustancia_ErrorDuplicadoO()
        {
            // Arrange
            var sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1",
                Cas = "123456",
                ListaCaq = "1A"
            };
    
            mockRepo.Setup(x => x.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia);
            mockRepo.Setup(x => x.SustanciasRepository.ValidarSustancia(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            mockRepo.Setup(x => x.SustanciasRepository.EditarSustancia(sustancia, 1, new List<string> { "Nombre" })).ReturnsAsync(ErroresSustancia.DatosDuplicados);
            var handler = new Sustancia.Commands.EditarSustancia.EditarSustanciaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new Sustancia.Commands.EditarSustancia.EditarSustanciaCommand { Sustancia = sustancia, IdSustancia = 1, ListaCambios = new List<string> { "Nombre" } }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task EliminarSustancia_Ok()
        {
            //Arrange
            var idSustancia = 1;
            var Sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1"
            };

            //Act
            EliminarSustanciaCommand command = new EliminarSustanciaCommand();
            command.IdSustancia = idSustancia;
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(It.IsAny<int>())).ReturnsAsync(Sustancia);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(idSustancia)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarSustanciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarSustancia_ObtenerSustanciaPorIdDevuelveError()
        {
            //Act
            EliminarSustanciaCommand command = new EliminarSustanciaCommand();
            command.IdSustancia = 1;
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(It.IsAny<int>())).ReturnsAsync(ErrorOr.Error.Failure());
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(command.IdSustancia)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarSustanciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarSustancia_Error()
        {
            //Arrange
            var idSustancia = 1;
            var Sustancia = new Dominio.Entidades.Sustancia
            {
                Id = 1,
                Nombre = "Sustancia 1"
            };

            //Act
            EliminarSustanciaCommand command = new EliminarSustanciaCommand();
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(idSustancia)).ReturnsAsync(Sustancia);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(0)).ReturnsAsync(ErrorOr.Error.Failure());
            var handler = new EliminarSustanciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
        }
       
        [Fact]
        public async Task EliminadoMasivoSustancias_DevuelveOk()
        {
            // Arrange
            var idsSustancias = new List<int> { 1, 2 };

            var sustancia1 = new Dominio.Entidades.Sustancia()
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "1234",
                ListaCaq = "Lista"
            };

            var sustancia2 = new Dominio.Entidades.Sustancia()
            {
                Id = 2,
                Nombre = "Nombre 2",
                Cas = "5464356",
                ListaCaq = "lista"
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia1);
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(2)).ReturnsAsync(sustancia2);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarSustanciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarSustanciasCommand { IdsSustancias = idsSustancias }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoSustancias_ErrorObtenerSustanciaPorId()
        {
            // Arrange
            var idsSustancias = new List<int> { 1, 2 };

            var sustancia1 = new Dominio.Entidades.Sustancia()
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "1234",
                ListaCaq = "Lista"
            };

            var sustancia2 = new Dominio.Entidades.Sustancia()
            {
                Id = 2,
                Nombre = "Nombre 2",
                Cas = "5464356",
                ListaCaq = "lista"
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(2)).ReturnsAsync(sustancia2);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarSustanciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarSustanciasCommand { IdsSustancias = idsSustancias }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoSustancias_ErrorEliminar()
        {
            // Arrange
            var idsBloquesComerciales = new List<int> { 1, 2 };

            var sustancia1 = new Dominio.Entidades.Sustancia()
            {
                Id = 1,
                Nombre = "Nombre 1",
                Cas = "1234",
                ListaCaq = "Lista"
            };

            var sustancia2 = new Dominio.Entidades.Sustancia()
            {
                Id = 2,
                Nombre = "Nombre 2",
                Cas = "45645",
                ListaCaq = "Lista"
            };

            var erroror = ErroresSustancia.NoEncontrado;

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(1)).ReturnsAsync(sustancia1);
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustanciaPorId(2)).ReturnsAsync(sustancia2);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(2)).ReturnsAsync(erroror);

            var handler = new EliminarSustanciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarSustanciasCommand { IdsSustancias = idsBloquesComerciales }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Sustancia.NoEncontrada", result.Errors[0].Code);
            Assert.Equal("Sustancia no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre 1",
                    Cas = "1234",
                    ListaCaq = "Lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveExistenDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() {
                new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre 1",
                    Cas = "1234",
                    ListaCaq = "Lista"
                },
                new Dominio.Entidades.Sustancia()
                {
                    Id = 2,
                    Nombre = "Nombre 1",
                    Cas = "1234",
                    ListaCaq = "Lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorValidarSustancia()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre 1",
                    Cas = "1234",
                    ListaCaq = "Lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ValidarSustancia(It.IsAny<int>(), "Nombre 1", "1234","Lista")).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task ImportarDatos_NombreInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre =  "".PadRight(301, 'a'),
                    Cas = "3453",
                    ListaCaq = "lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CasInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre =  "Nombre",
                    Cas = "3453".PadRight(101, 'a'),
                    ListaCaq = "lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ListaCaqInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre =  "Nombre",
                    Cas = "3453",
                    ListaCaq = "lista".PadRight(101, 'a')
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerSustancias()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre 1",
                    Cas = "3453",
                    ListaCaq = "lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarSustancias()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre",
                    Cas = "3453",
                    ListaCaq = "lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCrear()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Sustancia>() { new Dominio.Entidades.Sustancia()
                {
                    Id = 1,
                    Nombre = "Nombre 1" ,
                    Cas = "45634",
                    ListaCaq = "lista"
                }
            };

            mockRepo.Setup(repo => repo.SustanciasRepository.ObtenerSustancias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SustanciasRepository.EliminarSustancia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciasRepository.CrearSustancia(It.IsAny<Dominio.Entidades.Sustancia>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
    }
}
