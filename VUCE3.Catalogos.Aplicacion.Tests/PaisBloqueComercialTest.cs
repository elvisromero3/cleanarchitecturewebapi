using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Queries.ObtenerPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EditarPaisBloqueComercial;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.EliminarPaisBloqueComercial;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.CrearPaisBloqueComercial;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.PaisBloqueComercial.Commands.Importardatos;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class PaisBloqueComercialTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public PaisBloqueComercialTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerPaisBloqueComercial_Ok()
        {
            //Act
            var paisBloqueComercial = new List<Dominio.Entidades.PaisBloqueComercial>()
            {
                new Dominio.Entidades.PaisBloqueComercial()
                {
                    Id =1,
                    IdBloqueComercial = 1,
                    IdPais = 1

                }
            };

            ObtenerPaisBloqueComercialQuery obtenerPaisBloqueComercialQuery = new ObtenerPaisBloqueComercialQuery();
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(paisBloqueComercial);
            var handler = new ObtenerPaisBloqueComercialQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisBloqueComercialQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.PaisBloqueComercial>>(result.Value);
        }

        [Fact]
        public async Task ObtenerPaisBloqueComercialPorId_Ok()
        {
            //Arange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };

            //Act
            ObtenerPaisBloqueComercialPorIdQuery obtenerPaisBloqueComercialPorIdQuery = new ObtenerPaisBloqueComercialPorIdQuery();
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            var handler = new ObtenerPaisBloqueComercialPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisBloqueComercialPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarPaisBloqueComercial_Ok()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {

                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };

            var listaCambios = new List<string> { "IdBloqueComercial" };

            //Act
            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial= paisBloqueComercial, IdPaisBloqueComercial= paisBloqueComercial.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial( command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(paisBloqueComercial);
            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarPaisBloqueComercial_Error()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };

            var listaCambios = new List<string> { "IdBloqueComercial" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial = paisBloqueComercial, IdPaisBloqueComercial = paisBloqueComercial.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(errorIsError);
            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarPaisBloqueComercial_ErrorBloqueComercialNoEncontrado()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };
            var listaCambios = new List<string> { "IdBloqueComercial" };
            
            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial = paisBloqueComercial, IdPaisBloqueComercial = paisBloqueComercial.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(ErroresBloqueComercial.NoEncontrada);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(paisBloqueComercial);

            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Bloque comercial no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarPaisBloqueComercial_NoExiste()
        {
            //Arrange

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            var listaCambios = new List<string> { "IdBloqueComercial" };
            var errorIsError = ErroresPaisBloqueComercial.NoEncontrada;

            //Act

            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial = paisBloqueComercial, IdPaisBloqueComercial = paisBloqueComercial.Id, ListaCambios= listaCambios };

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(errorIsError);
            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Pais de bloque comercial no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercial_Ok()
        {
            //Arrange
            var id = 1;

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            //Act
            EliminarPaisBloqueComercialCommand command = new EliminarPaisBloqueComercialCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(id)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercial_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresPaisBloqueComercial.NoEncontrada;

            //Act
            EliminarPaisBloqueComercialCommand command = new EliminarPaisBloqueComercialCommand { Id = id};

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Pais de bloque comercial no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercial_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresPaisBloqueComercial.NoEncontrada;

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };

            //Act
            EliminarPaisBloqueComercialCommand command = new EliminarPaisBloqueComercialCommand { Id = id };

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(id)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("PaisBloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Pais de bloque comercial no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearPaisBloqueComercial_Ok()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Costa Rica"
            };

            CrearPaisBloqueComercialCommand command = new CrearPaisBloqueComercialCommand() { PaisBloqueComercial = paisBloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(It.IsAny<int>())).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial)).ReturnsAsync(paisBloqueComercial);

            var handler = new CrearPaisBloqueComercialCommandHandler(mockRepo.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.IdBloqueComercial);
        }

        [Fact]
        public async Task CrearPaisBloqueComercial_Error()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Costa Rica"
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            CrearPaisBloqueComercialCommand command = new CrearPaisBloqueComercialCommand() { PaisBloqueComercial = paisBloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(It.IsAny<int>())).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial)).ReturnsAsync(errorIsError);

            var handler = new CrearPaisBloqueComercialCommandHandler(mockRepo.Object);
            
            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearPaisBloqueComercial_ErrorBloqueComercialNoEncontrado()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Costa Rica"
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            CrearPaisBloqueComercialCommand command = new CrearPaisBloqueComercialCommand() { PaisBloqueComercial = paisBloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(ErroresBloqueComercial.NoEncontrada);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(It.IsAny<int>())).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial)).ReturnsAsync(errorIsError);

            var handler = new CrearPaisBloqueComercialCommandHandler(mockRepo.Object);
            
            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresPaisBloqueComercial.BloqueComercialNoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task CrearPaisBloqueComercial_ErrorPaisNoEncontrado()
        {
            //Arrange
            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            CrearPaisBloqueComercialCommand command = new CrearPaisBloqueComercialCommand() { PaisBloqueComercial = paisBloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(It.IsAny<int>())).ReturnsAsync(ErroresPaises.NoEncontrado);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial)).ReturnsAsync(errorIsError);

            var handler = new CrearPaisBloqueComercialCommandHandler(mockRepo.Object);
            
            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresPaisBloqueComercial.PaisNoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercials_DevuelveOk()
        {
            // Arrange
        
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 1 }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarPaisBloqueComercials_DevuelveError()
        {
            // Arrange
            var erroror = ErroresPaisBloqueComercial.NoEncontrada;

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(3)).ReturnsAsync(erroror);

            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 3 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarPaisBloqueComercials_CrearDevuelveError()
        {
            // Arrange

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1

            };
            var bloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Bloque 1"
            };
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Costa Rica"
            };

            CrearPaisBloqueComercialCommand command = new CrearPaisBloqueComercialCommand() { PaisBloqueComercial = paisBloqueComercial };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(It.IsAny<int>())).ReturnsAsync(bloqueComercial);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(It.IsAny<int>())).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(paisBloqueComercial.IdBloqueComercial, paisBloqueComercial.IdPais)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(paisBloqueComercial)).ReturnsAsync(paisBloqueComercial);

            var handler = new CrearPaisBloqueComercialCommandHandler(mockRepo.Object);

            //Act           
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("PaisBloqueComercial.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un país de bloque comercial con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarPaisBloqueComercials_EditarDevuelveExiste()
        {
            //Arrange

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1,
                IdPais = 1
            };


            var listaCambios = new List<string> { "IdBloqueComercial" };

            //Act
            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial = paisBloqueComercial, IdPaisBloqueComercial = paisBloqueComercial.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(paisBloqueComercial.IdBloqueComercial, paisBloqueComercial.IdPais)).ReturnsAsync(true);            
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(paisBloqueComercial);
            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("PaisBloqueComercial.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un país de bloque comercial con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarPaisBloqueComercials_EditarDevuelveError()
        {
            //Arrange

            var paisBloqueComercial = new Dominio.Entidades.PaisBloqueComercial()
            {
                Id = 1,
                IdBloqueComercial = 1111111,
                IdPais = 1
            };

           
            var listaCambios = new List<string> { "IdBloqueComercial" };

            //Act
            EditarPaisBloqueComercialCommand command = new EditarPaisBloqueComercialCommand { PaisBloqueComercial = paisBloqueComercial, IdPaisBloqueComercial = paisBloqueComercial.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercialPorId(1)).ReturnsAsync(paisBloqueComercial);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ValidarPaisBloqueComercial( paisBloqueComercial.IdBloqueComercial, paisBloqueComercial.IdPais)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ActualizarPaisBloqueComercial(command.IdPaisBloqueComercial, command.ListaCambios, command.PaisBloqueComercial)).ReturnsAsync(Error.Failure());
            var handler = new EditarPaisBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DuplicadoenArchivoOK()
        {
            // Arrange
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                     Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                }};


            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(Result.Deleted);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarError()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica2"
                }};



            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarTipoError()
        {
            // Arrange
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {   
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica2"
                }};



            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarCreated()
        {
            // Arrange
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1

                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                 Id = 2,
                 IdBloqueComercial = 1,
                 IdPais = 1
                }
            };

            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
                new Dominio.Entidades.BloqueComercial
                {
                   Id=1,
                   Nombre="bloque 1"
                },
                new Dominio.Entidades.BloqueComercial
                {
                    Id=2,
                    Nombre="bloque 2"
                }

           };

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 2",
                    Pais="Costa Rica"
                }};

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(PaisBloqueComercials[0])).ReturnsAsync(PaisBloqueComercials[0]);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatos_OK()
        {
            // Arrange
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = 1;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 2",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
                new Dominio.Entidades.BloqueComercial
                {
                   Id=1,
                   Nombre="bloque 1"
                },
                new Dominio.Entidades.BloqueComercial
                {
                    Id=2,
                    Nombre="bloque 2"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(PaisBloqueComercials[0])).ReturnsAsync(PaisBloqueComercials[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_ObtenerPais_Error()
        {
            // Arrange
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = 1;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial ="bloque 1",
                    Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial = "bloque 2",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Guatemala"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
                new Dominio.Entidades.BloqueComercial
                {
                   Id=1,
                   Nombre="bloque 1"
                },
                new Dominio.Entidades.BloqueComercial
                {
                    Id=2,
                    Nombre="bloque 2"
                }

           };
            // Act

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(PaisBloqueComercials[0])).ReturnsAsync(PaisBloqueComercials[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_ObtenerBloqueComercial_Error()
        {
            // Arrange
            var error = Error.Unexpected();

            var modo = 1;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                   BloqueComercial = "bloque 1",
                   Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial = "bloque 2",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }
            };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
            };
            // Act
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDuplicado()
        {
            // Arrange
            var error = Error.Unexpected();
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = 1;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                   BloqueComercial = "bloque 1",
                   Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial = "bloque 2",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }
           };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
                new Dominio.Entidades.BloqueComercial
                {
                   Id=1,
                   Nombre="bloque 1"
                },
                new Dominio.Entidades.BloqueComercial
                {
                    Id=2,
                    Nombre="bloque 2"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ValidarPaisBloqueComercial(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_InsertDatosError()
        {
            // Arrange
            var error = Error.Unexpected();
            var PaisBloqueComercials = new List<Dominio.Entidades.PaisBloqueComercial>
            {
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 1,
                    IdBloqueComercial = 1,
                    IdPais = 1
                },
                new Dominio.Entidades.PaisBloqueComercial
                {
                    Id = 2,
                    IdBloqueComercial = 1,
                    IdPais = 1
                }
            };

            var modo = 1;
            var datos = new List<ImportarPaisBloqueComercialCommandDto>
            {
                 new ImportarPaisBloqueComercialCommandDto
                {
                   BloqueComercial = "bloque 1",
                   Pais="Costa Rica"
                },
                new ImportarPaisBloqueComercialCommandDto
                {
                    BloqueComercial = "bloque 2",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }
           };

            var bloquesComerciales = new List<Dominio.Entidades.BloqueComercial>
            {
                new Dominio.Entidades.BloqueComercial
                {
                   Id=1,
                   Nombre="bloque 1"
                },
                new Dominio.Entidades.BloqueComercial
                {
                    Id=2,
                    Nombre="bloque 2"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.ObtenerPaisBloqueComercial()).ReturnsAsync(PaisBloqueComercials);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(bloquesComerciales);

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.CrearPaisBloqueComercial(It.IsAny<Dominio.Entidades.PaisBloqueComercial>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task BorradoMasivoPaisBloqueComercials_DevuelveOk()
        {
            // Arrange
            var idsPaisBloqueComercials = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 1}, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task BorradoMasivoPaisBloqueComercials_DevuelveError()
        {
            // Arrange
            var erroror = ErroresPaisBloqueComercial.PaisNoEncontrado;

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(3)).ReturnsAsync(erroror);

            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);


            // Act
            var result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 1}, default);
            result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 2 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
        [Fact]
        public async Task BorradoMasivoPaisBloqueComercials_DevuelveError_bloque()
        {
            // Arrange
            var erroror = ErroresPaisBloqueComercial.BloqueComercialNoEncontrado;

            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisBloqueComercialRepository.EliminarPaisBloqueComercial(3)).ReturnsAsync(erroror);

            var handler = new EliminarPaisBloqueComercialCommandHandler(mockRepo.Object);


            // Act
            var result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 1 }, default);
            result = await handler.Handle(new EliminarPaisBloqueComercialCommand { Id = 2 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

    }
}
