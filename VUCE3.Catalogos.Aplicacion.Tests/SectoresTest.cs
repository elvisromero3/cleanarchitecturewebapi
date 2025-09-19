using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.CrearSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EditarSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSector;
using VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectores;
using VUCE3.Catalogos.Aplicacion.Sectores.Queries.ObtenerSectorPorId;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Sectores.Commands.EliminarSectores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class SectoresTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        public SectoresTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerSectores_Ok()
        {
            //Act
            var sectores = new List<Sector>()
            {
                new Sector()
                {
                    Id =1,
                    Nombre = "Sector 1"
                }
            };

            ObtenerSectoresQuery obtenerSectoresQuery = new ObtenerSectoresQuery();
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(sectores);
            var handler = new ObtenerSectoresQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerSectoresQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Sector>>(result.Value);
        }

        [Fact]
        public async Task ObtenerSectorPorId_Ok()
        {
            //Arange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1"

            };

            //Act
            ObtenerSectorPorIdQuery obtenerSectorPorIdQuery = new ObtenerSectorPorIdQuery();
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            var handler = new ObtenerSectorPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerSectorPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearSector_Ok()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
                
            };

            //Act
            CrearSectorCommand command = new CrearSectorCommand() { Sector = sector };
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(sector)).ReturnsAsync(sector);

            var handler = new CrearSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Sector 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearSector_ErrorNombre()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "",
                Codigo = "Codigo 1"

            };

            //Act
            CrearSectorCommand command = new CrearSectorCommand() { Sector = sector };
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(sector)).ReturnsAsync(sector);

            var handler = new CrearSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Sector.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }
        [Fact]
        public async Task CrearSector_ErrorCodigo()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Nombre 1",
                Codigo = "Codigo 12345"

            };

            //Act
            CrearSectorCommand command = new CrearSectorCommand() { Sector = sector };
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(sector)).ReturnsAsync(sector);

            var handler = new CrearSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Sector.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El codigo es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 10 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearSector_Error()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearSectorCommand command = new CrearSectorCommand() { Sector = sector };
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(sector)).ReturnsAsync(errorIsError);

            var handler = new CrearSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearSector_DatosDuplicados()
        {
            //Arrange
            var sector = new Sector
            {
                Id = 1,
                Nombre = "Sector 1",
                Codigo = "Codigo 1"
            };

            var error = ErroresSector.SectorDatosDuplicados;

            //Act
            CrearSectorCommand command = new CrearSectorCommand() { Sector = sector };
            mockRepo.Setup(repo => repo.SectoresRepository.ValidarSector(command.Sector.Id, command.Sector.Nombre, command.Sector.Codigo)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(sector)).ReturnsAsync(error);
            var handler = new CrearSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ActualizarSector_Ok()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector",
                Codigo = "Codigo 1"
            };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(sector);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarSector_Error()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector",
                Codigo = "Codigo 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarSector_NoExiste()
        {
            //Arrange

            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector",
                Codigo = "Codigo 1"

            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };
            var errorIsError = ErroresSector.SectorNoEncontrado;

            //Act

            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Sector.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Sector no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarSector_DatosDuplicados()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Sector",
                Codigo = "Codigo 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresSector.SectorDatosDuplicados;

            //Act
            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.ValidarSector(command.IdSector, command.Sector.Nombre, command.Sector.Codigo)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ActualizarSector_NombreExcedeLimite()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio"
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresSector.SectorNombreInvalido;

            //Act
            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.ValidarSector(command.IdSector, command.Sector.Nombre, command.Sector.Codigo)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }
        [Fact]
        public async Task ActualizarSector_CodigoExcedeLimite()
        {
            //Arrange
            var sector = new Sector()
            {
                Id = 1,
                Nombre = "Nombre 1",
                Codigo = "Codigo 123456789"
            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresSector.SectorCodigoInvalido;

            //Act
            EditarSectorCommand command = new EditarSectorCommand(sector, sector.Id, listaCambios);

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(1)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.ValidarSector(command.IdSector, command.Sector.Nombre,command.Sector.Codigo)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task EliminarSector_Ok()
        {
            //Arrange
            var idSector = 1;
            var sector = new Sector
            {
                Id = 1,
                Nombre = "Sector 1"
            };

            //Act
            EliminarSectorCommand command = new EliminarSectorCommand();
            command.Id = idSector;
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(It.IsAny<int>())).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(idSector)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarSector_ObtenerSectorPorIdDevuelveError()
        {
            //Act
            EliminarSectorCommand command = new EliminarSectorCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(command.Id)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarSector_Error()
        {
            //Arrange
            var idSector = 1;
            var sector = new Sector
            {
                Id = 1,
                Nombre = "Sector 1"
            };

            //Act
            EliminarSectorCommand command = new EliminarSectorCommand();
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(idSector)).ReturnsAsync(sector);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(0)).ReturnsAsync(Error.Failure());
            var handler = new EliminarSectorCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_Ok()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };

            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDatosDuplicados()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                    Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                    Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                    Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                    Codigo = "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.SectorDatosDuplicados);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSector.SectorDatosDuplicados.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDatosDuplicadosArchivo()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSector.DatosDuplicadosArchivo.Code, result.FirstError.Code);
        }
        [Fact]
        public async Task ImportarDatos_ErrorEliminarModo2()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
           
        }
        [Fact]
        public async Task ImportarDatos_ErrorModo1()
        {
            //Arrange
            var modo = 1;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorNombre("Sector uno")).ReturnsAsync(datos[0]);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarModo2Error()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                     Codigo = "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo = "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.DatosDuplicadosArchivo);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_ErrorNombreExcedeLimite()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio",
                     Codigo= "Codigo 2"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo= "Codigo 1"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio",
                     Codigo= "Codigo 2"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo= "Codigo 1"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.SectorNombreInvalido);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSector.SectorNombreInvalido.Code, result.FirstError.Code);
        }
        [Fact]
        public async Task ImportarDatos_ErrorCodigoExcedeLimite()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Nombre 1",
                     Codigo= "Codigo 12346"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo= "Codigo 12345"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Nombre 1",
                     Codigo= "Codigo 12346"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                     Codigo= "Codigo 12345"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(ErroresSector.SectorNombreInvalido);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresSector.SectorCodigoInvalido.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_Error()
        {
            //Arrange
            var modo = 2;
            var datos = new List<Sector>() {
                new Sector() {
                    Nombre = "Sector uno",
                    Codigo= "Codigo 1"
                },
                new Sector()
                {
                    Nombre = "Sector dos",
                     Codigo= "Codigo 2"
                }
            };
            var datosDto = new List<ImportarSectorCommandDto>() {
                new ImportarSectorCommandDto() {
                    Nombre = "Sector uno",
                    Codigo= "Codigo 1"
                },
                new ImportarSectorCommandDto()
                {
                    Nombre = "Sector dos",
                    Codigo= "Codigo 2"
                }
            };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectores()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SectoresRepository.CrearSector(It.IsAny<Sector>())).ReturnsAsync(Error.Unexpected());
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(new ImportarDatosCommand { Datos = datosDto, Modo = modo }, default);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(Error.Unexpected().Code, result.Errors[0].Code);
        }

        [Fact]
        public async Task EliminarDatosMasivos_Ok()
        {
            //Arrange
            var items = new List<int> { 1, 2, 3 };
            var command = new EliminarSectoresCommand { IdsSectores = items };
            mockRepo.Setup(repo => repo.SectoresRepository.ObtenerSectorPorId(It.IsAny<int>())).ReturnsAsync(new Sector());
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            var handler = new EliminarSectoresCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarDatosMasivos_Error()
        {
            //Arrange
            var items = new List<int> { 1, 2, 3 };
            var command = new EliminarSectoresCommand { IdsSectores = items };
            mockRepo.Setup(repo => repo.SectoresRepository.EliminarSector(It.IsAny<int>())).ReturnsAsync(ErroresSector.SectorNoEncontrado);
            var handler = new EliminarSectoresCommandHandler(mockRepo.Object);
            //Act
            var result = await handler.Handle(command, default);
            //Assert
            Assert.True(result.IsError);
        }
    }
}
