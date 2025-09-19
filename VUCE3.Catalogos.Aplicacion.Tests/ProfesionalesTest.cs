using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionalPorId;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EditarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.CrearProfesional;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ProfesionalesTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        private Mock<IAccesoGestionUsuariosService> mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        public ProfesionalesTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();

        }

        [Fact]
        public async Task ObtenerProfesionales_Ok()
        {

            //Act

            var empresas = new List<Dominio.Entidades.Empresa>()
            {
                new Dominio.Entidades.Empresa()
                {

                   Id = 1,
                   Nombre = "CAF",
                   NumeroIdentificacion = "123456",
                   IdTipoIdentificacion = 'F',
                   Profesional = new Dominio.Entidades.Profesional {Id = 1}

                },
                new Dominio.Entidades.Empresa()
                {

                   Id = 2,
                   Nombre = "Daka",
                   NumeroIdentificacion = "789654",
                   IdTipoIdentificacion = 'F',
                   Profesional = new Dominio.Entidades.Profesional {Id = 1}

                }
            };
            
            var profesionales = new List<Dominio.Entidades.Profesional>()
            {
                new Dominio.Entidades.Profesional()
                {
                    Id = 1,
                    Nombre = "Pedro Perez",
                    IdTipoIdentificacion = 'F',
                    NumeroIdentificacion = "123456",
                    Profesion = "Ingeniero",
                    Email = "test@test.com",
                    CodigoRegente = "123456",
                    Activo = true,
                    Empresas = empresas
                },
                 new Dominio.Entidades.Profesional()
                {
                    Id = 2,
                    Nombre = "Pedro Sanchez",
                    IdTipoIdentificacion = 'J',
                    NumeroIdentificacion = "789123",
                    Profesion = "Ingeniero",
                    Email = "test@test.com",
                    CodigoRegente = "789123",
                    Activo = true,
                    Empresas = empresas
                }
            };

            ObtenerProfesionalesQuery obtenerProfesionalQuery = new ObtenerProfesionalesQuery();
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(profesionales);
            var handler = new ObtenerProfesionalesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProfesionalQuery, default);           

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(result.Value.Count, result.Value?.Count);
            
        }
     

        [Fact]
        public async Task ObtenerProfesionalPorId_Ok()
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

            //Act
            ObtenerProfesionalPorIdQuery obtenerProfesionalPorIdQuery = new ObtenerProfesionalPorIdQuery();
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            var handler = new ObtenerProfesionalPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProfesionalPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_Ok()
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

            var listaCambios = new List<string> { "Jose Perez" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);            
        }

        [Fact]
        public async Task ActualizarProfesional_IdInstitucionError()
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
                IdInstitucion = 0,
                Activo = true
            };

            var listaCambios = new List<string> { "IdInstitucion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorEmailTamano()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "Email" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorEmailFormato()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "testtest.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "Email" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorEmailFormatoPuntos()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test...com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "Email" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_Error()
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

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarProfesional_NoExiste()
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

            var listaCambios = new List<string> { "Pedro Casillero" };
            var errorIsError = ErroresProfesionales.NoEncontrado;

            //Act

            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Regente no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorTipoIdentificacion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'P',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "IdTipoIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorNroIdentificacionTipo()
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

            var listaCambios = new List<string> { "NumeroIdentificacion", "IdTipoIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorNroIdentificacionTipoFisicaComienza0()
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

            var listaCambios = new List<string> { "NumeroIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorNroIdentificacion()
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

            var listaCambios = new List<string> { "NumeroIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorTamañoNombre()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez Pedro Perez Pedro Perez Pedro PerezPedro PerezPedro PerezPedro PerezPedro PerezPedro Perez Pedro Perez Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "Nombre"};

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorNumeroIdentificacionDimex()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro ASSz",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "NumeroIdentificacion", "IdTipoIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }


        [Fact]
        public async Task ActualizarProfesional_ErrorTamañoProfesion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456789",
                Profesion = "Ingeniero IngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true
            };

            var listaCambios = new List<string> { "Profesion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorTamañoCodigoRegente()
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
                CodigoRegente = "123456111111111111111",
                Activo = true
            };

            var listaCambios = new List<string> { "CodigoRegente" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_ErrorDuplicado()
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
                CodigoRegente = "123456111",
                IdInstitucion = 3,
                Activo = true
            };

            var listaCambios = new List<string> { "IdInstitucion", "IdTipoIdentificacion", "NumeroIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(Error.Unexpected());
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProfesional_DuplicadoTrue()
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
                CodigoRegente = "123456111",
                IdInstitucion = 3,
                Activo = true
            };

            var listaCambios = new List<string> { "IdInstitucion", "IdTipoIdentificacion", "NumeroIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id,  profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task EliminarProfesional_Ok()
        {
            //Arrange
            var idProfesional = 1;
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

            //Act
            EliminarProfesionalCommand command = new EliminarProfesionalCommand();
            command.Id = idProfesional;
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(idProfesional)).ReturnsAsync(profesional); 
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(idProfesional)).ReturnsAsync(Result.Deleted); 
            var handler = new EliminarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProfesional_ObtenerProfesionalPorIdDevuelveError()
        {
            //Arrange
            var idProfesional = 1;
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

            //Act
            EliminarProfesionalCommand command = new EliminarProfesionalCommand();
            command.Id = idProfesional;
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(idProfesional)).ReturnsAsync(Error.Failure());
            var handler = new EliminarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarProfesional_Error()
        {
            //Arrange
            var idProfesional = 1;
            var errorIsError = ErroresProfesionales.NoEncontrado;
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

            //Act
            EliminarProfesionalCommand command = new EliminarProfesionalCommand();
            command.Id = idProfesional;
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(idProfesional)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(idProfesional)).ReturnsAsync(errorIsError);
            var handler = new EliminarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Regente no encontrado", result.FirstError.Description);
        }         

        [Fact]
        public async Task CrearProfesional_Ok()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456789012",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id,  profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Pedro Perez", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearProfesional_IdInstitucion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 0,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            
        }

        [Fact]
        public async Task CrearProfesional_ErrorTamanoEmail()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.TamanoEmail", result.FirstError.Code);
            Assert.Equal("El email debe tener máximo 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTamanoNumeroIdentificacion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "testtesttes@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.TamanoNroIdentificacion", result.FirstError.Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProfesional_ErrorFormatoEmail()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "testtest.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true
            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.EmailInvalido", result.FirstError.Code);
            Assert.Equal("Email inválido", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProfesional_ErrorFormatoEmailPuntos()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'F',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.....com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.EmailInvalido", result.FirstError.Code);
            Assert.Equal("Email inválido", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTipoIdentificacion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'P',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTipoIdentificacionFisica()
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
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorNumeroIdentificacionFisicaComienza0()
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
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProfesionales.TipoIdentificacionFisicaComienza0.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTipoIdentificacionOtras()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123457894562556",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTamañoNombre()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez  Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "1234578946",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTamañoProfesion()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "1234578946",
                Profesion = "Ingeniero IngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngenieroIngeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorTamañoCodigoRegente()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "1234578946",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "12345644568954789654785",
                Activo = true,
                IdInstitucion = 1

            };

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_ErrorProfesionalExiste()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion = 1
            };

            var errorIsError = Error.Unexpected();

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(errorIsError);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProfesional_Error()
        {
            //Arrange
            var profesional = new Dominio.Entidades.Profesional()
            {
                Id = 1,
                Nombre = "Pedro Perez",
                IdTipoIdentificacion = 'D',
                NumeroIdentificacion = "123456",
                Profesion = "Ingeniero",
                Email = "test@test.com",
                CodigoRegente = "123456",
                Activo = true,
                IdInstitucion = 1
            };

            var errorIsError = Error.Unexpected();

            //Act
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(errorIsError);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarProfesionales_DevuelveOk()
        {
            // Arrange
            var idsProfesionales = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarProfesionalesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProfesionalesCommand { IdsProfesionales = idsProfesionales }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProfesionales_DevuelveError()
        {
            // Arrange
            var idsProfesionales = new List<int> { 1, 2, 3 };
            var erroror = ErroresProfesionales.NoEncontrado;

            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(3)).ReturnsAsync(erroror);

            var handler = new EliminarProfesionalesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProfesionalesCommand { IdsProfesionales = idsProfesionales }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "123456789012",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

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
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };


            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.False(result.IsError);
        }
        
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoNumeroIdentificacion()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion= '1', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'A',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                },
                 new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'A',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

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
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };


            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_ObtenerProfesionalDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };
            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",

                }
            };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarProfesionalDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() {Id=-1, IdTipoIdentificacion = 'F', CodigoRegente = "A" } };
            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",

                }
            };
           
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(-1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        
        [Fact]
        public async Task ImportarDatos_ObtenerInstitucionesDevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

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
  
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearProfesionalDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };
            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",

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
            var institucion = new List<InstitucionDto>() { new InstitucionDto() 
            {
                Id= 1,
                Nombre = "MINCEXP"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(Error.Failure());
          
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_CrearProfesional_TipoIdentificacion_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };
            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "DimexP"
                }
             };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEXP"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_CrearProfesional_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', CodigoRegente = "A" } };
            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion = -1

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
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarProfesionalesCrear_DevuelveError()
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
                IdInstitucion = 1,
                Activo = true

            };

            //Act
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(true);
            CrearProfesionalCommand command = new CrearProfesionalCommand() { Profesional = profesional };
            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(profesional)).ReturnsAsync(profesional);

            var handler = new CrearProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);


            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ValidarProfesionalesEditar_DevuelveExiste()
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

            var listaCambios = new List<string> { "IdTipoIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Regente.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ValidarProfesionalesEditar_DevuelveError()
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

            var listaCambios = new List<string> { "IdTipoIdentificacion" };

            //Act
            EditarProfesionalCommand command = new EditarProfesionalCommand(profesional, profesional.Id, listaCambios);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(profesional.Id, profesional.NumeroIdentificacion, profesional.IdInstitucion)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionalPorId(1)).ReturnsAsync(profesional);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios)).ReturnsAsync(profesional);
            var handler = new EditarProfesionalCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarProfesionalesDevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'F', NumeroIdentificacion = "3RTFF1234", CodigoRegente = "ER4", IdInstitucion = 1 } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {                    
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "3RTFF1234",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =1

                }
            };
            
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.ValidarProfesional(0, "3RTFF1234", 1)).ReturnsAsync(true);           
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.Equal("Regente.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_TipoIdentificacionError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'P',
                    NumeroIdentificacion = "3RTFF",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                },
             };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);            
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoIdentificacionFisica()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "3RTFF12345",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);            
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoIdentificacionFisicaComienza0()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'F',
                    NumeroIdentificacion = "023456789",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 1,
                Nombre = "MINCEX"

            } };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoIdentificacionOtras()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123454555",
                    Profesion ="Consultor",
                    Email = "procomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TamañoEmail()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123",
                    Profesion ="Consultor",
                    Email = "procomerprocomerprocomerprocomerprocomerprocomerprocomerprocomerprocomerprocomerprocomer@procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EmailInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "A",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123",
                    Profesion ="Consultor",
                    Email = "procomer.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TamañoNombre()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123",
                    Profesion ="Consultor",
                    Email = "procomer@test.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);            
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TamañoProfesion()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "AAAAAAA",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123",
                    Profesion ="ConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultorConsultor",
                    Email = "procomer@test.com",
                    CodigoRegente ="ER4",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TamañoCodigoRegente()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Profesional>() { new Dominio.Entidades.Profesional() { IdTipoIdentificacion = 'P', CodigoRegente = "A" } };

            var datosImportar = new List<ImportarProfesionalCommandDto>()
            {
                new ImportarProfesionalCommandDto()
                {
                    Nombre = "AAAAAAA",
                    Activo  = true,
                    TipoIdentificacion = 'D',
                    NumeroIdentificacion = "3RTFF123",
                    Profesion ="Consultor",
                    Email = "procomer@test.com",
                    CodigoRegente ="ER412111452145214521552222",
                    Institucion = "MINCEX",
                    IdInstitucion =-1

                }
            };
            var tiposIdentificacion = new List<TipoIdentificacionDto>()
            {
                new TipoIdentificacionDto()
                {
                    Id = 1,
                    Nombre = "Física"
                },
                new TipoIdentificacionDto()
                {
                    Id = 2,
                    Nombre = "Jurídica"
                },
                new TipoIdentificacionDto()
                {
                    Id = 3,
                    Nombre = "Dimex"
                },
                new TipoIdentificacionDto()
                {
                    Id = 4,
                    Nombre = "Pasaporte"
                }
             };
            var institucion = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };

            mockRepo.Setup(repo => repo.ProfesionalesRepository.ObtenerProfesionales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProfesionalesRepository.EliminarProfesional(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProfesionalesRepository.CrearProfesional(It.IsAny<Dominio.Entidades.Profesional>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datosImportar }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
