using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.CrearCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EditarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristica;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EliminarCaracteristicas;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicaPorId;
using VUCE3.Catalogos.Aplicacion.Caracteristicas.Queries.ObtenerCaracteristicas;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.CrearCasa;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasas;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;

using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CaracteristicasTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        private Mock<IAccesoGestionUsuariosService> mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();

        public CaracteristicasTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        }

        [Fact]
        public async Task ObtenerCaracteristicas_Ok()
        {
            //Act
            var caracteristicas = new List<Caracteristica>()
            {
                new Caracteristica()
                {
                    Id =1,
                    IdInstitucion = 1,
                    Nombre = "Caracteristica 1"
                }
            };

            ObtenerCaracteristicasQuery obtenerCaracteristicasQuery = new ObtenerCaracteristicasQuery();
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            var handler = new ObtenerCaracteristicasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCaracteristicasQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Caracteristica>>(result.Value);
        }

        [Fact]
        public async Task ObtenerCaracteristicaPorId_Ok()
        {
            //Arange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            //Act
            ObtenerCaracteristicaPorIdQuery obtenerCaracteristicaPorIdQuery = new ObtenerCaracteristicaPorIdQuery();
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            var handler = new ObtenerCaracteristicaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCaracteristicaPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearCaracteristica_Ok()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica 1"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(caracteristica);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Caracteristica 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearCaracteristica_InstitucionNoEncontrada()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "MINCEX"
                }
            };

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(caracteristica);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.CaracteristicaInstitucionInvalida", result.Errors[0].Code);
        }

        [Fact]
        public async Task CrearCaracteristica_ErrorNombre()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = ""

            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(caracteristica);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.NombreInvalido", result.FirstError.Code);
        }


        [Fact]
        public async Task CrearCaracteristica_ErrorNombreMaximo()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "El Servicio Nacional Integrado de Administración Aduanera y Tributaria para los diferentes países del gremio que ahora debe tener mas de 150 caracteres para fallar"

            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(caracteristica);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.NombreInvalido", result.FirstError.Code);
        }

        [Fact]
        public async Task CrearCaracteristica_Error()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica 1"

            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };
            var errorIsError = Error.Unexpected();

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(errorIsError);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCaracteristica_DatosDuplicados()
        {
            //Arrange
            var caracteristica = new Caracteristica
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica 1"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };var error = ErroresCaracteristica.CaracteristicaDatosDuplicados;

            
            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(command.Caracteristica.Id, command.Caracteristica.IdInstitucion, command.Caracteristica.Nombre)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(error);
            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task CrearCaracteristica_ErrorInstitucion()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Valorrrr5555"

            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            //Act
            CrearCaracteristicaCommand command = new CrearCaracteristicaCommand() { Caracteristica = caracteristica };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(caracteristica)).ReturnsAsync(caracteristica);

            var handler = new CrearCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.CaracteristicaInstitucionInvalida", result.FirstError.Code);
        }


        [Fact]
        public async Task ActualizarCaracteristica_Ok()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica",
            };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios)).ReturnsAsync(caracteristica);
            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCaracteristica_Error()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica"
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarCaracteristica_NoExiste()
        {
            //Arrange

            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica",

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresCaracteristica.CaracteristicaNoEncontrada;

            //Act

            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Característica no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCaracteristica_InstitucionNoEncontradaDCA()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var listaCambios = new List<string> { "Nombre", "IdInstitucion" };
            var errorIsError = ErroresCaracteristica.CaracteristicaDatosDuplicados;

            //Act
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(command.IdCaracteristica, command.IdCaracteristica, command.Caracteristica.Nombre)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios)).ReturnsAsync(errorIsError);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.CaracteristicaInstitucionInvalida", result.FirstError.Code);
            Assert.Equal("Institución no es DCA o DIPOA", result.FirstError.Description);
        }


        [Fact]
        public async Task ActualizarCaracteristica_DatosDuplicadosError()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var listaCambios = new List<string> { "Nombre", "IdInstitucion" };
            var errorIsError = ErroresCaracteristica.CaracteristicaDatosDuplicados;

            //Act
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(Error.Failure());
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarCaracteristica_DatosDuplicados()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 5,
                Nombre = "Caracteristica"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var listaCambios = new List<string> { "Nombre", "IdInstitucion" };
            var errorIsError = ErroresCaracteristica.CaracteristicaDatosDuplicados;

            //Act
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ActualizarCaracteristica_NombreExcedeLimite()
        {
            //Arrange
            var caracteristica = new Caracteristica()
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = new string('A',151)
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresCaracteristica.CaracteristicaNombreInvalido;
            EditarCaracteristicaCommand command = new EditarCaracteristicaCommand(caracteristica, caracteristica.Id, listaCambios);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(1)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(command.IdCaracteristica, command.Caracteristica.IdInstitucion, command.Caracteristica.Nombre)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarCaracteristicaCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(errorIsError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task EliminarCaracteristica_Ok()
        {
            //Arrange
            var idCaracteristica = 1;
            var caracteristica = new Caracteristica
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            //Act
            EliminarCaracteristicaCommand command = new EliminarCaracteristicaCommand();
            command.Id = idCaracteristica;
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(It.IsAny<int>())).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(idCaracteristica)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarCaracteristicaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristica_ObtenerCaracteristicaPorIdDevuelveError()
        {
            //Act
            EliminarCaracteristicaCommand command = new EliminarCaracteristicaCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(command.Id)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarCaracteristicaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristica_Error()
        {
            //Arrange
            var idCaracteristica = 1;
            var caracteristica = new Caracteristica
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1"
            };

            //Act
            EliminarCaracteristicaCommand command = new EliminarCaracteristicaCommand();
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(idCaracteristica)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(0)).ReturnsAsync(Error.Failure());
            var handler = new EliminarCaracteristicaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCaracteristica_ErrorTipoProducto()
        {
            //Arrange
            var idCaracteristica = 1;
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristicaTipoProducto = new List<Dominio.Entidades.CaracteristicaTipoProducto>() { new Dominio.Entidades.CaracteristicaTipoProducto()
            {
                  Id =1,
                  TipoProducto = tipoproducto,
            }
            };


            var caracteristica = new Caracteristica
            {
                Id = 1,
                IdInstitucion = 1,
                Nombre = "Caracteristica 1",
                CaracteristicaTipoProductos = caracteristicaTipoProducto
            };

            //Act
            EliminarCaracteristicaCommand command = new EliminarCaracteristicaCommand();
            command.Id = idCaracteristica;
            var error = ErroresCaracteristica.TipoProductoRelacionado;

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicaPorId(idCaracteristica)).ReturnsAsync(caracteristica);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(idCaracteristica)).ReturnsAsync(error);

            var handler = new EliminarCaracteristicaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.TipoProductoRelacionado", result.FirstError.Code);
            Assert.Equal("Existe caracteristica de tipo producto relacionadas", result.FirstError.Description);
        }



        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Caracteristica>() { new Caracteristica() { IdInstitucion = 5, Nombre = "A" } };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "Caracteristica1",
                    IdInstitucion = 5
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones()).ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = 5, Nombre = "DCA" } });
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(It.IsAny<Caracteristica>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Caracteristica>() { new Caracteristica() { IdInstitucion = 5, Nombre = "A" } };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "Caracteristica1"
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones()).ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = 5, Nombre = "DCA" } });
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);
            
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Caracteristica.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerCaracteristicasDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Caracteristica>() { new Caracteristica() { IdInstitucion = ConstantesInstituciones.DCA, Nombre = "DCA" } };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "Caracteristica1"
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones()).ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = ConstantesInstituciones.DCA, Nombre = "DCA" } });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminararacteristicaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Caracteristica>() { new Caracteristica() { IdInstitucion = ConstantesInstituciones.DCA, Nombre = "A" } };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "Caracteristica1"
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones()).ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = ConstantesInstituciones.DCA, Nombre = "DCA" } });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearCaracteristicaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Caracteristica>() { new Caracteristica() { IdInstitucion = ConstantesInstituciones.DCA, Nombre = "A" } };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "Caracteristica1"
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones())
                .ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = ConstantesInstituciones.DCA, Nombre = "DCA" } });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(It.IsAny<Caracteristica>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorNombreMuyLargo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Caracteristica>() {
                new Caracteristica()
                {
                    IdInstitucion = ConstantesInstituciones.DCA,
                    Nombre = new string('A', 151)
                }
            };
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = new string('A', 151)
                }
            };

            mockAccesoGestionUsuariosService.Setup(service => service.ObtenerInstituciones()).ReturnsAsync(new List<InstitucionDto> { new InstitucionDto { Id = ConstantesInstituciones.DCA, Nombre = "DCA" } });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.CrearCaracteristica(It.IsAny<Caracteristica>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDatoDuplicadoArchivo()
        {
            // Arrange
            var modo = 1;
            var datosDto = new List<ImportarDatosCaracteristicaDto>
            {
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "A"
                },
                new ImportarDatosCaracteristicaDto
                {
                    Institucion = "DCA",
                    Nombre = "A"
                }
            };
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ValidarCaracteristica(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosDto }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristicas_DevuelveOk()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarCaracteristicasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCaracteristicasCommand { Ids = ids }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristicas_DevuelveError()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.EliminarCaracteristica(3)).ReturnsAsync(erroror);

            var handler = new EliminarCaracteristicasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCaracteristicasCommand { Ids = ids }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
    }
}
