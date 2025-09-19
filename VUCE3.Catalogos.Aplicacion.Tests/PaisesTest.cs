using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaisPorId;
using VUCE3.Catalogos.Aplicacion.Pais.Queries.ObtenerPaises;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.CrearPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EditarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPais;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.ImportarDatos;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Pais.Commands.EliminarPaises;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class PaisesTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        public PaisesTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerPaises_Ok()
        {
            //Act
            var paises = new List<Dominio.Entidades.Pais>()
            {
                new Dominio.Entidades.Pais()
                {
                    Id =1,
                    Nombre = "Venezuela"                    
                }
            };

            ObtenerPaisesQuery obtenerPaisQuery = new ObtenerPaisesQuery();
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            var handler = new ObtenerPaisesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Pais>>(result.Value);
        }

        [Fact]
        public async Task ObtenerPaisPorId_Ok()
        {
            //Arange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"                
            };

            //Act
            ObtenerPaisPorIdQuery obtenerPaisPorIdQuery = new ObtenerPaisPorIdQuery();
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            var handler = new ObtenerPaisPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarPais_Ok()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "España",
                CodigoC3 = "ESP",
                CodigoA2 = "ES",
                CodigoNumerico = "123"
            };

            var listaCambios = new List<string> { "España" };

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(pais);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarPais_Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"

            };
            
            var listaCambios = new List<string> { "España" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarPais_NoExiste()
        {
            //Arrange

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"

            };

            var listaCambios = new List<string> { "Casillero" };
            var errorIsError = ErroresPaises.NoEncontrado;

            //Act

            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Pais.NoEncontrado", result.FirstError.Code);
            Assert.Equal("País no encontrado", result.FirstError.Description);
        }

     

        [Fact]
        public async Task EliminarPais_Ok()
        {
            //Arrange
            var idPais = 1;
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"

            };

            //Act
            EliminarPaisCommand command = new EliminarPaisCommand();
            command.Id = idPais;
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(idPais)).ReturnsAsync(Result.Deleted); 
            var handler = new EliminarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarPais_ObtenerPaisPorIdDevuelveError()
        {
            //Act
            EliminarPaisCommand command = new EliminarPaisCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(Error.Failure());
            var handler = new EliminarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarPais_NoExiste()
        {
            //Arrange
            var idPais = 1;
            var errorIsError = ErroresPaises.NoEncontrado;
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"

            };

            //Act
            EliminarPaisCommand command = new EliminarPaisCommand();
            command.Id = idPais;
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(idPais)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(idPais)).ReturnsAsync(errorIsError);
            var handler = new EliminarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Pais.NoEncontrado", result.FirstError.Code);
            Assert.Equal("País no encontrado", result.FirstError.Description);
        }         

        [Fact]
        public async Task CrearPais_Ok()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };            
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(pais);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Venezuela", result.Value.Nombre);            
        }

        [Fact]
        public async Task CrearPais_Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var errorIsError = Error.Unexpected();

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(errorIsError);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoCodigoA2Archivo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Pais>() {
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO" },
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica1", CodigoA2="A2", CodigoNumerico="124", CodigoC3="ISA" },
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoCodigoNumericoArchivo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Pais>() {
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO"  },
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica1", CodigoA2="A3", CodigoNumerico="123", CodigoC3="ISA"  },
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoNombreArchivo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Pais>() {
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO"  },
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A3", CodigoNumerico="124", CodigoC3="ISA"  },
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoCodigoC3Archivo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Pais>() {
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO"  },
                new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica1", CodigoA2="A3", CodigoNumerico="124", CodigoC3="ISO"  },
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerpaisesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Pais>() { new Dominio.Entidades.Pais() { Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO" } };

           
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(Error.Failure());

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
            var datos = new List<Dominio.Entidades.Pais>() { new Dominio.Entidades.Pais() {  Id=1, Nombre = "Costa Rica", CodigoA2="A2", CodigoNumerico="123", CodigoC3="ISO"  } };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(It.IsAny<int>())).ReturnsAsync(Error.Failure());

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

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarPaises_DevuelveOk()
        {
            // Arrange
            var idsPaises = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarPaisesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarPaisesCommand { IdsPaises = idsPaises }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarPaises_DevuelveError()
        {
            // Arrange
            var idsPaises = new List<int> { 1, 2, 3 };
            var erroror = ErroresPaises.NoEncontrado;

            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(3)).ReturnsAsync(erroror);

            var handler = new EliminarPaisesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarPaisesCommand { IdsPaises  = idsPaises }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarPaisesCrear_DevuelveErrorNombre()
        {

            // Arrange            

            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre,pais.CodigoA2,pais.CodigoNumerico,pais.CodigoC3)).ReturnsAsync(paisExiste);            
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(pais);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearPaisCommand { Pais = pais }, default);


            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesCrear_DevuelveErrorCodigoA2()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(pais);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearPaisCommand { Pais = pais }, default);


            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesCrear_DevuelveErrorCodigoC3()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VEN",
                CodigoA2 = "VA",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(pais);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearPaisCommand { Pais = pais }, default);


            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesCrear_DevuelveErrorCodigoNumerico()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VAN",
                CodigoA2 = "VA",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(pais);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearPaisCommand { Pais = pais }, default);


            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesEditar_DevuelveExisteNombre()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            IEnumerable<string> listaCambios = new List<string> { "Nombre" };
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(pais, pais.Id, listaCambios)).ReturnsAsync(pais);

            var handler = new EditarPaisCommandHandler(mockRepo.Object);            

            // Act
            var result = await handler.Handle(new EditarPaisCommand(pais, pais.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesEditar_DevuelveExisteCodigoA2()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            IEnumerable<string> listaCambios = new List<string> { "CodigoA2" };
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(pais, pais.Id, listaCambios)).ReturnsAsync(pais);

            var handler = new EditarPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarPaisCommand(pais, pais.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesEditar_DevuelveExisteCodigoC3()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VEN",
                CodigoA2 = "VA",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            IEnumerable<string> listaCambios = new List<string> { "CodigoC3" };
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(pais, pais.Id, listaCambios)).ReturnsAsync(pais);

            var handler = new EditarPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarPaisCommand(pais, pais.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesEditar_DevuelveExisteCodigoNumerico()
        {

            // Arrange            
            var paisExiste = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela1",
                CodigoC3 = "VAN",
                CodigoA2 = "VA",
                CodigoNumerico = "123"
            };

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            IEnumerable<string> listaCambios = new List<string> { "CodigoNumerico" };
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(paisExiste);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(pais, pais.Id, listaCambios)).ReturnsAsync(pais);

            var handler = new EditarPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarPaisCommand(pais, pais.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarPaisesEditar_DevuelveError()
        {

            // Arrange            

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Venezuela",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            IEnumerable<string> listaCambios = new List<string> { "CodigoA2" };
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(pais.Id, pais.Nombre, pais.CodigoA2, pais.CodigoNumerico, pais.CodigoC3)).ReturnsAsync(Error.Failure());            
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(pais, pais.Id, listaCambios)).ReturnsAsync(pais);

            var handler = new EditarPaisCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarPaisCommand(pais, pais.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarPaisesDevuelveErrorNombre()
        {
            // Arrange
            var modo = 1;
            var paisExiste = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var pais = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(datos[0].Id, datos[0].Nombre, datos[0].CodigoA2, datos[0].CodigoNumerico, datos[0].CodigoC3)).ReturnsAsync(paisExiste);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarPaisesDevuelveErrorCodigoA2()
        {
            // Arrange
            var modo = 1;
            var paisExiste = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica2", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var pais = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(datos[0].Id, datos[0].Nombre, datos[0].CodigoA2, datos[0].CodigoNumerico, datos[0].CodigoC3)).ReturnsAsync(paisExiste);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarPaisesDevuelveErrorCodigoC3()
        {
            // Arrange
            var modo = 1;
            var paisExiste = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica2", CodigoA2 = "AA", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var pais = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(datos[0].Id, datos[0].Nombre, datos[0].CodigoA2, datos[0].CodigoNumerico, datos[0].CodigoC3)).ReturnsAsync(paisExiste);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarPaisesDevuelveErrorCodigoNumerico()
        {
            // Arrange
            var modo = 1;
            var paisExiste = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica2", CodigoA2 = "AA", CodigoNumerico = "123", CodigoC3 = "ISA" };
            var pais = new Dominio.Entidades.Pais() { Id = 1, Nombre = "Costa Rica", CodigoA2 = "AB", CodigoNumerico = "123", CodigoC3 = "ISO" };
            var datos = new List<Dominio.Entidades.Pais>() { pais };

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.ValidarPais(datos[0].Id, datos[0].Nombre, datos[0].CodigoA2, datos[0].CodigoNumerico, datos[0].CodigoC3)).ReturnsAsync(paisExiste);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearPais_DevuelveValidacionNombreError()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var error = ErroresPaises.ValidacionNombre;

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(error);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearPais_DevuelveValidacionCodigoA2Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE1111",
                CodigoNumerico = "123"
            };

            var error = ErroresPaises.CodigoA2ExcedeLimite;

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(error);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearPais_DevuelveValidacionCodigoNumeroError()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "1234"
            };

            var error = ErroresPaises.CodigoNumericoExcedeLimite;

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(error);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearPais_DevuelveValidacionCodigoC3Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN1",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var error = ErroresPaises.CodigoC3ExcedeLimite;

            //Act
            CrearPaisCommand command = new CrearPaisCommand() { Pais = pais };
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(pais)).ReturnsAsync(error);

            var handler = new CrearPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarPais_DevuelveValidacionNombreError()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresPaises.ValidacionNombre;

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarPais_DevuelveValidacionCodigoA2Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE1111",
                CodigoNumerico = "123"
            };

            var listaCambios = new List<string> { "CodigoA2" };

            var error = ErroresPaises.CodigoA2ExcedeLimite;

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarPais_DevuelveValidacionCodigoNumeroError()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "1234"
            };

            var listaCambios = new List<string> { "CodigoNumerico" };

            var error = ErroresPaises.CodigoNumericoExcedeLimite;

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarPais_DevuelveValidacionCodigoC3Error()
        {
            //Arrange
            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN1",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var listaCambios = new List<string> { "CodigoC3" };

            var error = ErroresPaises.CodigoC3ExcedeLimite;

            //Act
            EditarPaisCommand command = new EditarPaisCommand(pais, pais.Id, listaCambios);

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.PaisesRepository.ActualizarPais(command.Pais, command.IdPais, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarPaisCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreError()
        {
            // Arrange
            var modo = 2;

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais 11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            var error = ErroresPaises.ValidacionNombre;

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoA2Error()
        {
            // Arrange
            var modo = 2;

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE1111",
                CodigoNumerico = "123"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            var error = ErroresPaises.CodigoA2ExcedeLimite;

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoNumeroError()
        {
            // Arrange
            var modo = 2;

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN",
                CodigoA2 = "VE",
                CodigoNumerico = "1234"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            var error = ErroresPaises.CodigoNumericoExcedeLimite;

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoC3Error()
        {
            // Arrange
            var modo = 2;

            var pais = new Dominio.Entidades.Pais()
            {
                Id = 1,
                Nombre = "Pais",
                CodigoC3 = "VEN1",
                CodigoA2 = "VE",
                CodigoNumerico = "123"
            };

            var datos = new List<Dominio.Entidades.Pais>() { pais };

            var error = ErroresPaises.CodigoC3ExcedeLimite;

            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.PaisesRepository.EliminarPais(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.CrearPais(It.IsAny<Dominio.Entidades.Pais>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

    }
}
