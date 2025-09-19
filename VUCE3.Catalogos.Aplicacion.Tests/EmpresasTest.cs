using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresaPorId;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EditarEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresa;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.CrearEmpresa;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresas;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Aplicacion.Empresa.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class EmpresasTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        public EmpresasTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerEmpresas_Ok()
        {
            //Act

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresas = new List<Dominio.Entidades.Empresa>()
            {
                new Dominio.Entidades.Empresa()
                {
              
                   Id = 1,
                   Nombre = "CAF",
                   NumeroIdentificacion = "123456",
                   IdTipoIdentificacion = 'F',
                   Profesional = new Dominio.Entidades.Profesional { Id = 1 }

                },
                new Dominio.Entidades.Empresa()
                {

                   Id = 2,
                   Nombre = "Daka",
                   NumeroIdentificacion = "789654",
                   IdTipoIdentificacion = 'F',
                   Profesional = profesional

                }
            };

            ObtenerEmpresasQuery obtenerEmpresasQuery = new ObtenerEmpresasQuery();
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresas()).ReturnsAsync(empresas);
            var handler = new ObtenerEmpresasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerEmpresasQuery, default);
            
            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
        }

        [Fact]
        public async Task ObtenerEmpresaPorId_Ok()
        {
            //Arange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true

            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }

            };

            //Act
            ObtenerEmpresaPorIdQuery obtenerEmpresaPorIdQuery = new ObtenerEmpresaPorIdQuery();
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            var handler = new ObtenerEmpresaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerEmpresaPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);            
        }

        [Fact]
        public async Task ActualizarEmpresa_Ok()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "Nombre", "NumeroIdentificacion", "IdTipoIdentificacion" };

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(empresa);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarEmpresa_IdProfesionalError()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                IdProfesional = 0
            };

            var listaCambios = new List<string> { "IdProfesional"};

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(empresa);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresas.IdProfesionalInvalido", result.FirstError.Code);
            Assert.Equal("El identificador de profesional es un campo requerido y su valor debe ser mayor a 0", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarEmpresa_Error()
        {
            //Arrange

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1,
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "IdProfesional" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarEmpresa_NoExiste()
        {
            //Arrange

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "CAF" };
            var errorIsError = ErroresEmpresas.NoEncontrado;

            //Act

            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Empresa no encontrada", result.FirstError.Description);
        }     

        [Fact]
        public async Task EliminarEmpresa_Ok()
        {
            //Arrange
            var idEmpresa = 1;
            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            //Act
            EliminarEmpresaCommand command = new EliminarEmpresaCommand();
            command.Id = idEmpresa;
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(idEmpresa)).ReturnsAsync(empresa); 
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(idEmpresa)).ReturnsAsync(Result.Deleted); 

            var handler = new EliminarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarEmpresa_Error()
        {
            //Arrange
            var idEmpresa = 1;
            var errorIsError = ErroresEmpresas.NoEncontrado;
            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };
            //Act
            EliminarEmpresaCommand command = new EliminarEmpresaCommand();
            command.Id = idEmpresa;
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(idEmpresa)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(idEmpresa)).ReturnsAsync(errorIsError);
            var handler = new EliminarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Empresa no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarEmpresa_ObtenerEmpresaPorIdDevuelveError()
        {
            //Arrange
            var idEmpresa = 1;
            var errorIsError = ErroresEmpresas.NoEncontrado;

            //Act
            EliminarEmpresaCommand command = new EliminarEmpresaCommand();
            command.Id = idEmpresa;
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(idEmpresa)).ReturnsAsync(errorIsError);
            var handler = new EliminarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Empresa no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearEmpresa_Ok()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(empresa);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("CAF", result.Value.Nombre);            
        }

        [Fact]
        public async Task CrearEmpresa_ProfesionalNoExiste()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = profesional.Id,
                Profesional = profesional
            };

            var errorIsError = ErroresProfesionales.NoEncontrado;

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.EmpresasRepository.ValidarEmpresa(empresa.Id,empresa.IdTipoIdentificacion,empresa.NumeroIdentificacion,empresa.IdProfesional)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(empresa.Profesional.Id)).ReturnsAsync(errorIsError);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearEmpresa_Error()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(new Dominio.Entidades.Profesional { Id = 1 });
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(errorIsError);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearEmpresa_ErrorIdProfesional()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                IdProfesional = 0 
            };

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };            
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(empresa);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresas.IdProfesionalInvalido", result.Errors[0].Code);
            Assert.Equal("El identificador de profesional es un campo requerido y su valor debe ser mayor a 0", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearEmpresa_ErrorNombre()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(empresa);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresa.NombreExcedeLimite", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearEmpresa_ErrorNumeroIdentificacion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "abc",
                NumeroIdentificacion = "",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(empresa);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ValidacionNumeroIdentificacion", result.Errors[0].Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarEmpresas_DevuelveOk()
        {
            // Arrange
            var idsEmpresas = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarEmpresasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEmpresasCommand { IdsEmpresas = idsEmpresas }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarEmpresas_DevuelveError()
        {
            // Arrange
            var idsEmpresas = new List<int> { 1, 2, 3 };
            var erroror = ErroresEmpresas.NoEncontrado;

            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(3)).ReturnsAsync(erroror);

            var handler = new EliminarEmpresasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEmpresasCommand { IdsEmpresas = idsEmpresas }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
           var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456789",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "123456789012",
                    TipoIdentificacion = 'D'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ExisteDuplicado()
        {
            // Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456789",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerEmpresasDevuelveError()
        {
            // Arrangea
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            }}
            ;

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarEmpresaDevuelveError()
        {
            // Arrange 

            var modo = 2;
            var errorNoEncontrado = ErroresEmpresas.NoEncontrado;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                } 
            };

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(errorNoEncontrado);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearEmpresaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
            {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_TipoidentificadorDevuelveErrorServicio()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
            {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_TipoidentificadorDevuelveErrorBuscar()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
            {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Dimex3"
                }
             };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 1;
            var datosDevueltos = new List<Dominio.Entidades.Empresa>()
            {
                new Dominio.Entidades.Empresa() { Nombre = "A", IdProfesional=1,IdTipoIdentificacion='A',NumeroIdentificacion="1234" },
                new Dominio.Entidades.Empresa() { Nombre = "A", IdProfesional=1,IdTipoIdentificacion='A',NumeroIdentificacion="1234" }
            };

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto() { Nombre = "A", IdProfesional=1,TipoIdentificacion='A',NumeroIdentificacion="1234" },
                new ImportarEmpresaCommandDto() { Nombre = "A", IdProfesional=1,TipoIdentificacion='A',NumeroIdentificacion="1234" }
            };

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresas()).ReturnsAsync(datosDevueltos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datosDevueltos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicadosNoRemplazar()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
            {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456789",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "Home Depot",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.ValidarEmpresa(0, 'D', "RFG45", 1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarEmpresaCrear_DevuelveError()
        {

            // Arrange

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            mockRepo.Setup(repo => repo.EmpresasRepository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(empresa);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new CrearEmpresaCommand { Empresa = empresa }, default);


            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresas.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Empresa asociada al regente ya existe", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarEmpresaEditar_DevuelveExiste()
        {

            // Arrange            

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 },
                IdProfesional = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "NumeroIdentificacion" };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(empresa, empresa.Id, listaCambios)).ReturnsAsync(empresa);

            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarEmpresaCommand(empresa, empresa.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Empresas.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Empresa asociada al regente ya existe", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarEmpresaEditar_DevuelveError()
        {

            // Arrange            

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 },
                IdProfesional = 1
            };

            IEnumerable<string> listaCambios = new List<string> { "NumeroIdentificacion" };
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ValidarEmpresa(empresa.Id, empresa.IdTipoIdentificacion, empresa.NumeroIdentificacion, empresa.IdProfesional)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(empresa, empresa.Id, listaCambios)).ReturnsAsync(empresa);

            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarEmpresaCommand(empresa, empresa.Id, listaCambios), default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearEmpresa_DevuelveValidacionNombreError()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true

            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            var error = ErroresEmpresas.EmpresaNombreExcedeLimite;

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(error);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearEmpresa_DevuelveValidacionNumeroIdentificacionError()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true

            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "1234561234561223",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            var error = ErroresEmpresas.NumeroIdentificacionExcedeLimite;

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(error);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearEmpresa_DevuelveValidacionTipoIdentificacionError()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true

            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'A',
                IdProfesional = 1
            };

            var error = ErroresEmpresas.TipoIdentificacionNoEncontrado;

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(error);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearEmpresa_DevuelveValidacionTipoIdentificacionErrorComienza0()
        {
            //Arrange

            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "023456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true

            };

            var empresa = new Dominio.Entidades.Empresa()
            {

                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "023456789",
                IdTipoIdentificacion = 'F',
                IdProfesional = 1
            };

            var error = ErroresEmpresas.TipoIdentificacionFisicaComienza0;

            //Act
            CrearEmpresaCommand command = new CrearEmpresaCommand() { Empresa = empresa };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(empresa)).ReturnsAsync(error);

            var handler = new CrearEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarEmpresa_DevuelveValidacionNombreError()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresEmpresas.EmpresaNombreExcedeLimite;

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarEmpresa_DevuelveValidacionNumeroIdentificacionError()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456123456123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "Nombre", "NumeroIdentificacion" };

            var error = ErroresEmpresas.NumeroIdentificacionExcedeLimite;

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarEmpresa_DevuelveValidacionNumeroIdentificacionErrorComienza0()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "023456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "NumeroIdentificacion" };

            var error = ErroresEmpresas.TipoIdentificacionFisicaComienza0;

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarEmpresa_DevuelveValidacionTipoIdentificacionError()
        {
            //Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'A',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var listaCambios = new List<string> { "IdTipoIdentificacion" };

            var error = ErroresEmpresas.TipoIdentificacionNoEncontrado;

            //Act
            EditarEmpresaCommand command = new EditarEmpresaCommand(empresa, empresa.Id, listaCambios);

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(1)).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios)).ReturnsAsync(error);
            var handler = new EditarEmpresaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreError()
        {
            // Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var modo = 2;
            
            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };
            
            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                    NumeroIdentificacion = "RFG45",
                    TipoIdentificacion = 'D'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            var error = ErroresEmpresas.EmpresaNombreExcedeLimite;

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNumeroIdentificacionError()
        {
            // Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF A",
                NumeroIdentificacion = "123456123456123456",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var modo = 2;

            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456123456123456",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "CAF A",
                    NumeroIdentificacion = "123456123456123456",
                    TipoIdentificacion = 'D'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            var error = ErroresEmpresas.NumeroIdentificacionExcedeLimite;

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNumeroIdentificacionErrorComienza0()
        {
            // Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF A",
                NumeroIdentificacion = "023456789",
                IdTipoIdentificacion = 'F',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var modo = 2;

            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "023456789",
                    IdTipoIdentificacion = 'F',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "CAF A",
                    NumeroIdentificacion = "023456789",
                    TipoIdentificacion = 'F'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Fisica"
                }
             };

            var error = ErroresEmpresas.TipoIdentificacionFisicaComienza0;

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionTipoIdentificacionError()
        {
            // Arrange
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1,
                Nombre = "CAF A",
                NumeroIdentificacion = "123456",
                IdTipoIdentificacion = 'Z',
                Profesional = new Dominio.Entidades.Profesional { Id = 1 }
            };

            var modo = 2;

            var datos = new List<Dominio.Entidades.Empresa>() { new Dominio.Entidades.Empresa()
                {
                    Id = 1,
                    Nombre = "CAF",
                    NumeroIdentificacion = "123456123456123456",
                    IdTipoIdentificacion = 'Z',
                    Profesional = new Dominio.Entidades.Profesional { Id = 1 }
                }
            };

            var datosImportar = new List<ImportarEmpresaCommandDto>()
            {
                new ImportarEmpresaCommandDto()
                {
                    IdProfesional = 1,
                    Nombre = "CAF A",
                    NumeroIdentificacion = "123456",
                    TipoIdentificacion = 'Z'
                }
            };

            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                }
             };

            var error = ErroresEmpresas.TipoIdentificacionNoEncontrado;

            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresasPorIdProfesional(1)).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EmpresasRepository.EliminarEmpresa(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EmpresasRepository.CrearEmpresa(It.IsAny<Dominio.Entidades.Empresa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

    }
}
