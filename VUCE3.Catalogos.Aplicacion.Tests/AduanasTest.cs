using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.CrearAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EditarAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminaAduana;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminarAduanas;
using VUCE3.Catalogos.Aplicacion.Aduanas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanaPorId;
using VUCE3.Catalogos.Aplicacion.Aduanas.Queries.ObtenerAduanas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class AduanasTest
    {
        private Mock<IUnitOfWork> mockRepo;
        private Mock<IAccesoGestionUsuariosService> mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();

        public AduanasTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        }

        [Fact]
        public async Task ObtenerAduanas_Ok()
        {
            //Arange
            var lstAduanas = new List<Aduana>()
            {
                new Aduana{
                    Id = 1,
                    Nombre = "Aduana 1"
                },
                new Aduana{
                    Id = 2,
                    Nombre = "Aduana 2"
                },
            };

            //Act
            ObtenerAduanasQuery obtenerAduanasQuery = new ObtenerAduanasQuery();
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(lstAduanas);
            var handler = new ObtenerAduanasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerAduanasQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ObtenerDivisionesPorId_Ok()
        {
            //Arange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Act
            ObtenerAduanaPorIdQuery obtenerAduanaPorIdQuery = new ObtenerAduanaPorIdQuery();
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(aduana);
            var handler = new ObtenerAduanaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerAduanaPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearAduana_Ok()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Act
            CrearAduanaCommand command = new CrearAduanaCommand() { Aduana = aduana };            
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(aduana)).ReturnsAsync(aduana);
            var handler = new CrearAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Aduana 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearAduana_ErrorNombre()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = ""
            };

            //Act
            CrearAduanaCommand command = new CrearAduanaCommand() { Aduana = aduana };
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(aduana)).ReturnsAsync(aduana);
            var handler = new CrearAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Aduana.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearAduana_Error()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var error = ErrorOr.Error.Unexpected();

            //Act
            CrearAduanaCommand command = new CrearAduanaCommand() { Aduana = aduana };
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(aduana)).ReturnsAsync(error);
            var handler = new CrearAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.FirstError.Code);
            Assert.Equal("An unexpected error has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearAduana_DatosDuplicados()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            var error = ErroresAduana.DatosDuplicados;

            //Act
            CrearAduanaCommand command = new CrearAduanaCommand() { Aduana = aduana };
            mockRepo.Setup(repo => repo.AduanasRepository.ValidarAduana(command.Aduana)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(aduana)).ReturnsAsync(ErroresAduana.DatosDuplicados);
            var handler = new CrearAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task CrearAduana_NombreExcedeLimite()
        {
            //Arrange
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio"
            };

            var error = ErroresAduana.AduanaNombreInvalido;

            //Act
            CrearAduanaCommand command = new CrearAduanaCommand() { Aduana = aduana };
            mockRepo.Setup(repo => repo.AduanasRepository.ValidarAduana(command.Aduana)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(aduana)).ReturnsAsync(ErroresAduana.AduanaNombreInvalido);
            var handler = new CrearAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task EliminarAduana_Ok()
        {
            //Arrange
            var idAduana = 1;
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };

            //Act
            EliminarAduanaCommand command = new EliminarAduanaCommand();
            command.Id = idAduana;
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(It.IsAny<int>())).ReturnsAsync(aduana);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ExisteRelacionAduanaInstitucionesAutorizadas(1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(idAduana)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarAduanaCommandHandler(mockRepo.Object,mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarAduana_ObtenerAduanaPorIdDevuelveError()
        {
            //Act
            EliminarAduanaCommand command = new EliminarAduanaCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(It.IsAny<int>())).ReturnsAsync(ErrorOr.Error.Failure());

            var handler = new EliminarAduanaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminarAduana_ExisteRelacionInstitucionesDevuelveError()
        {
            //Act
            EliminarAduanaCommand command = new EliminarAduanaCommand();
            command.Id = 1;
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };
            var errorFalloServices = ErroresAduana.FalloServicioAccesoGestionUsuario;
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(It.IsAny<int>())).ReturnsAsync(aduana);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ExisteRelacionAduanaInstitucionesAutorizadas(1)).ReturnsAsync(errorFalloServices);
            var handler = new EliminarAduanaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminarAduana_ExisteRelacionInstitucionesDevuelveTrue()
        {
            //Act
            EliminarAduanaCommand command = new EliminarAduanaCommand();
            command.Id = 1;
            var aduana = new Aduana
            {
                Id = 1,
                Nombre = "Aduana 1"
            };
            var institucionesAutorizadas = new AduanaInstitucionUsuarioDto
            {
                IdInstitucionAutorizada = 1,
                Id =1,
                IdAduana =1,
            };

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(It.IsAny<int>())).ReturnsAsync(aduana);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ExisteRelacionAduanaInstitucionesAutorizadas(1)).ReturnsAsync(true);
            var handler = new EliminarAduanaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarAduana_Error()
        {
            //Arrange
            var idAduana = 1;
            var errorIsError = ErroresAduana.NoEncontrada;

            //Act
            EliminarAduanaCommand command = new EliminarAduanaCommand();
            command.Id = idAduana;
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(idAduana)).ReturnsAsync(errorIsError);
            var handler = new EliminarAduanaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Aduana.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Aduana no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarAduana_Ok()
        {
            //Arrange
            var aduana = new Aduana()
            {
                Id = 1,
                Nombre = "Aduana",
            };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarAduanaCommand command = new EditarAduanaCommand(aduana, aduana.Id, listaCambios);

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(aduana);
            mockRepo.Setup(repo => repo.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios)).ReturnsAsync(aduana);
            var handler = new EditarAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarAduana_Error()
        {
            //Arrange
            var aduana = new Dominio.Entidades.Aduana()
            {
                Id = 1,
                Nombre = "Aduana",

            };

            var listaCambios = new List<string> { "Aduana" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarAduanaCommand command = new EditarAduanaCommand(aduana, aduana.Id, listaCambios);

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(aduana);
            mockRepo.Setup(repo => repo.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarAduana_NoExiste()
        {
            //Arrange

            var aduana = new Dominio.Entidades.Aduana()
            {
                Id = 1,
                Nombre = "Aduana",

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresAduana.NoEncontrada;

            //Act

            EditarAduanaCommand command = new EditarAduanaCommand(aduana, aduana.Id, listaCambios);

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Aduana.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Aduana no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarAduana_DatosDuplicados()
        {
            //Arrange
            var aduana = new Aduana()
            {
                Id = 1,
                Nombre = "Aduana",

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresAduana.DatosDuplicados;

            //Act
            EditarAduanaCommand command = new EditarAduanaCommand(aduana, aduana.Id, listaCambios);

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(aduana);
            mockRepo.Setup(repo => repo.AduanasRepository.ValidarAduana(command.Aduana)).ReturnsAsync(true);   
            mockRepo.Setup(repo => repo.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ActualizarAduana_NombreExcedeLimite()
        {
            //Arrange
            var aduana = new Aduana()
            {
                Id = 1,
                Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio"
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresAduana.AduanaNombreInvalido;

            //Act
            EditarAduanaCommand command = new EditarAduanaCommand(aduana, aduana.Id, listaCambios);

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanaPorId(1)).ReturnsAsync(aduana);
            mockRepo.Setup(repo => repo.AduanasRepository.ValidarAduana(command.Aduana)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.AduanasRepository.ActualizarAduana(command.Aduana, command.IdAduana, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarAduanaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Aduana>() { new Aduana()
            {
                Id = 1,
                Nombre = "Aduana"
            }
            };
            var aduana = new Aduana()
            {
                Id=1,
                Nombre = "Aduana 1"
            };

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(It.IsAny<Aduana>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Aduana>() {
                new Aduana()
                {
                    Id = 1,
                    Nombre = "Aduana"
                }           
            };

            var error = ErroresAduana.DatosDuplicados;
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.AduanasRepository.ValidarAduana(datos[0])).ReturnsAsync(true);
           
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Aduana.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una aduana con los datos proporcionados", result.FirstError.Description);
        }
        [Fact]
        public async Task ImportarDatos_ObtenerpaisesDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Aduana>() { new Aduana()
            {
                Id = 1,
                Nombre= "Aduana"
            }
            };

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(ErrorOr.Error.Failure());

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
            var datos = new List<Dominio.Entidades.Aduana>() { new Dominio.Entidades.Aduana()
            {
                Id = -1,
                Nombre= "Aduana"
                }
            };
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(-1)).ReturnsAsync(Error.Failure());

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
            var datos = new List<Aduana>() { new Aduana()
            {
                Id = -1,
                Nombre= "Aduana"
            }
            };
            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(It.IsAny<Aduana>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatosExcedenLimiteNombre_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Aduana>() { new Aduana()
            {
                Id = 1,
                Nombre= "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio"
            }
            };

            var error = ErroresAduana.AduanaNombreInvalido;

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Aduana>()
            {
                new Dominio.Entidades.Aduana() { Nombre = "A" },
                new Dominio.Entidades.Aduana() { Nombre = "A" }
            };

            mockRepo.Setup(repo => repo.AduanasRepository.ObtenerAduanas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.AduanasRepository.CrearAduana(It.IsAny<Dominio.Entidades.Aduana>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoAduanas_DevuelveOk()
        {
            // Arrange
            var idsAduanas = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarAduanasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarAduanasCommand { IdsAduanas = idsAduanas }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoAduanas_DevuelveError()
        {
            // Arrange
            var idsAduanas = new List<int> { 1, 2, 3 };
            var erroror = ErroresAduana.NoEncontrada;

            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.AduanasRepository.EliminarAduana(3)).ReturnsAsync(erroror);

            var handler = new EliminarAduanasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarAduanasCommand { IdsAduanas = idsAduanas }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

    }
}
