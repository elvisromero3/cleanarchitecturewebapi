using ErrorOr;
using Moq;
using System;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.CrearExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EditarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.EliminarExcepcionMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionesMorosidad;
using VUCE3.Catalogos.Aplicacion.ExcepcionesMorosidad.Queries.ObtenerExcepcionMorosidadPorId;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ExcepcionesMorosidadTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        private Mock<ITramitesService> mockTramites = new Mock<ITramitesService>();

        public ExcepcionesMorosidadTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockTramites = new Mock<ITramitesService>();
        }

        [Fact]
        public async Task ObtenerExcepcionesMorosidad_Ok()
        {
            //Act
            var excepciones = new List<ExcepcionMorosidad>()
            {
                new ExcepcionMorosidad()
                {
                    Id =1
                }
            };

            ObtenerExcepcionesMorosidadQuery obtenerExcepcionesMorosidadQuery = new ObtenerExcepcionesMorosidadQuery();
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(excepciones);
            var handler = new ObtenerExcepcionesMorosidadQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerExcepcionesMorosidadQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<ExcepcionMorosidad>>(result.Value);
        }

        [Fact]
        public async Task ObtenerExcepcionMorosidadPorId_Ok()
        {
            //Arange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            //Act
            ObtenerExcepcionMorosidadPorIdQuery obtenerExcepcionMorosidadPorIdQuery = new ObtenerExcepcionMorosidadPorIdQuery();
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            var handler = new ObtenerExcepcionMorosidadPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerExcepcionMorosidadPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_Ok()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "NumeroIdentificacionEmpresa", "TipoIdentificacionEmpresa" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            //  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorFechaInicioFechaVencimiento()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2024, 09, 09)
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "FechaInicio", "FechaVencimiento" };

            //Act
            var command = new EditarExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(excepcion.Id, listaCambios, excepcion)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento", result.Errors[0].Code);
            Assert.Equal("La fecha de inicio debe ser menor a la fecha de vencimiento", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorFechaInicioMenorAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = new DateTime(2024, 09, 09),
                FechaVencimiento = DateTime.Now,
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "FechaInicio" };

            //Act
            var command = new EditarExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(excepcion.Id, listaCambios, excepcion)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.FechaInicioMenorAlDia", result.Errors[0].Code);
            Assert.Equal("La fecha de inicio es un campo requerido, debe ser una fecha mayor o igual a hoy", result.Errors[0].Description);
        }

        // Fecha de inicio igual a hoy
        [Fact]
        public async Task ActualizarExcepcionMorosidad_OkFechaInicioIgualAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 09, 09),
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "FechaInicio" };

            //Act
            var command = new EditarExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(excepcion.Id, listaCambios, excepcion)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.True(result.Value.Item2.IdEstado == ConstantesEstadosExcepcionMorosidad.ACTIVA);
        }

        // Fecha de inicio mayor a hoy
        [Fact]
        public async Task ActualizarExcepcionMorosidad_OkFechaInicioMayorAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now.AddDays(3),
                FechaVencimiento = DateTime.Now.AddDays(20),
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "FechaInicio" };

            //Act
            var command = new EditarExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(excepcion.Id, listaCambios, excepcion)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.True(result.Value.Item2.IdEstado == ConstantesEstadosExcepcionMorosidad.PROGRAMADA);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_Error()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "IdEmpresa" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(errorIsError);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_NoExiste()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "IdEmpresa" };
            var errorIsError = ErroresExcepcionMorosidad.NoEncontrada;

            //Act

            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(errorIsError);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Excepción morosidad no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_NoExisteEmpresa()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "IdEmpresa" };
            var errorIsError = ErroresExcepcionMorosidad.NoEncontrada;

            //Act

            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            // mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(errorIsError);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorTipoIdentificacion()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'W',
                NumeroIdentificacionEmpresa = "123456789",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "NumeroIdentificacionEmpresa", "TipoIdentificacionEmpresa" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            //  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorExcedeLimite()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "1234567898555515454544545",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "NumeroIdentificacionEmpresa", "TipoIdentificacionEmpresa" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            //  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarExcepcionMorosidad_Comienza0()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "012345678",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "NumeroIdentificacionEmpresa", "TipoIdentificacionEmpresa" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            //  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorTramiteNTRegimen()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                IdTipoTramite = 2,
                IdRegimen = null,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "IdTipoTramite", "IdRegimen" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);//  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_ErrorTramiteRegimen()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                IdTipoTramite = 1,
                IdRegimen = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "IdTipoTramite" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);//  mockRepo.Setup(repo => repo.EmpresasRepository.ObtenerEmpresaPorId(It.IsAny<int>())).ReturnsAsync(empresa);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_Ok()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones"
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorCrear()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now.AddDays(3),
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones"                
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(ErrorOr.Error.Failure());
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);

        }

        [Fact]
        public async Task CrearExcepcionMorosidad_Error()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now.AddDays(3),
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones"
            };
            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var error = ErrorOr.Error.Unexpected();

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(error);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.FirstError.Code);
            Assert.Equal("An unexpected error has occurred.", result.FirstError.Description);
        }
        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorNumeroIdentificacion()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "12346789",
            };
            var error = ErrorOr.Error.Unexpected();

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(error);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);


            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.NumeroIdentificacionExcedeLimite", result.Errors[0].Code);
            Assert.Equal("El tamaño del número de identificación es incorrecto", result.Errors[0].Description);
        }
        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorNumeroIdentificacionComienza0()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "012346789",
            };
            var error = ErrorOr.Error.Unexpected();

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(error);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);


            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.TipoIdentificacionFisicaComienza0", result.Errors[0].Code);
            Assert.Equal("El número de identificación de tipo física no puede comenzar con 0", result.Errors[0].Description);
        }
        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorTipoIdentificacion()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'W',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now.AddDays(3),
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 3
            };
            var error = ErrorOr.Error.Unexpected();

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(error);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);


            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.TipoIdentificacionNoEncontrado", result.Errors[0].Code);
            Assert.Equal("Tipo Identificación no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorFechaInicioFechaVencimiento()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2024, 09, 09)
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento", result.Errors[0].Code);
            Assert.Equal("La fecha de inicio debe ser menor a la fecha de vencimiento", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorFechaInicioMenorAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = new DateTime(2024, 09, 09),
                FechaVencimiento = DateTime.Now,
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.FechaInicioMenorAlDia", result.Errors[0].Code);
            Assert.Equal("La fecha de inicio es un campo requerido, debe ser una fecha mayor o igual a hoy", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_TamañoNombre()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa "
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.NombreEmpresaInvalido", result.Errors[0].Code);
            Assert.Equal("El nombre de empresa es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_TamañoObservaciones()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones " +
                "Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones "
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.ObservacionesInvalidas", result.Errors[0].Code);
            Assert.Equal("El campo observaciones es requerido, debe tener un tamaño mayor a 0 y un máximo de 250 caracteres", result.Errors[0].Description);
        }

        // Fecha de inicio igual a hoy
        [Fact]
        public async Task CrearExcepcionMorosidad_OkFechaInicioIgualAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones"
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.True(result.Value.IdEstado == ConstantesEstadosExcepcionMorosidad.ACTIVA);
        }

        // Fecha de inicio mayor a hoy
        [Fact]
        public async Task CrearExcepcionMorosidad_OkFechaInicioMayorAHoy()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = new DateTime(2026, 12, 31),
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones"
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.True(result.Value.IdEstado == ConstantesEstadosExcepcionMorosidad.PROGRAMADA);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorTipoTramiteRegimen()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                IdTipoTramite = 1,
                IdSubtipoTramite = 1, 
                IdRegimen = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 1
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);            
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_ErrorTipoTramiteNTRegimen()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                IdTipoTramite = 2,
                IdSubtipoTramite = 1,
                IdRegimen = null,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 1
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarExcepcionMorosidad_Ok()
        {
            //Arrange
            var idexcepcion = 1;
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
            };

            //Act
            EliminarExcepcionMorosidadCommand command = new EliminarExcepcionMorosidadCommand();
            command.Id = idexcepcion;
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(It.IsAny<int>())).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(idexcepcion)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);
            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarExcepcionMorosidad_ObtenerExcepcionMorosidadPorIdDevuelveError()
        {
            //Act
            EliminarExcepcionMorosidadCommand command = new EliminarExcepcionMorosidadCommand();
            command.Id = 1;
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new EliminarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarExcepcionMorosidad_Error()
        {
            //Arrange
            var idexcepcion = 1;
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
            };

            //Act
            EliminarExcepcionMorosidadCommand command = new EliminarExcepcionMorosidadCommand();
            command.Id = idexcepcion;
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(It.IsAny<int>())).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(idexcepcion)).ReturnsAsync(Error.Failure());

            var handler = new EliminarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoExcepcionesMorosidad_DevuelveOk()
        {
            // Arrange
            var idsExcepcionesMorosidad = new List<int> { 1, 2 };

            var excepcion1 = new ExcepcionMorosidad()
            {
                Id = 1,
                IdEstado = 3
            };

            var excepcion2 = new ExcepcionMorosidad()
            {
                Id = 2,
                IdEstado = 3
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion1);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(2)).ReturnsAsync(excepcion2);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarExcepcionesMorosidadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarExcepcionesMorosidadCommand { IdsExcepcionesMorosidad = idsExcepcionesMorosidad }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoxcepcioneMorosidad_ErrorObtenerExcepcionMorosidadPorId()
        {
            // Arrange
            var idsExcepcionesMorosidad = new List<int> { 1, 2 };

            var excepcion1 = new ExcepcionMorosidad()
            {
                Id = 1,
                IdEstado = 3
            };

            var excepcion2 = new ExcepcionMorosidad()
            {
                Id = 2,
                IdEstado = 3
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(2)).ReturnsAsync(excepcion2);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarExcepcionesMorosidadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarExcepcionesMorosidadCommand { IdsExcepcionesMorosidad = idsExcepcionesMorosidad }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoExcepcionesMorosidad_ErrorEliminar()
        {
            // Arrange
            var idsExcepcionesMorosidad = new List<int> { 1, 2 };

            var excepcion1 = new ExcepcionMorosidad()
            {
                Id = 1,
                IdEstado = 3
            };

            var excepcion2 = new ExcepcionMorosidad()
            {
                Id = 2,
                IdEstado = 3
            };

            var erroror = ErroresExcepcionMorosidad.NoEncontrada;

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion1);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(2)).ReturnsAsync(excepcion2);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(2)).ReturnsAsync(erroror);

            var handler = new EliminarExcepcionesMorosidadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarExcepcionesMorosidadCommand { IdsExcepcionesMorosidad = idsExcepcionesMorosidad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.NoEncontrada", result.Errors[0].Code);
            Assert.Equal("Excepción morosidad no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminadoMasivoExcepcionesMorosidad_EliminacionNoPermitida()
        {
            // Arrange
            var idsExcepcionesMorosidad = new List<int> { 1, 2 };

            var excepcion1 = new ExcepcionMorosidad()
            {
                Id = 1,
                IdEstado = 1
            };

            var excepcion2 = new ExcepcionMorosidad()
            {
                Id = 2,
                IdEstado = 3
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion1);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(2)).ReturnsAsync(excepcion2);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarExcepcionesMorosidadCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarExcepcionesMorosidadCommand { IdsExcepcionesMorosidad = idsExcepcionesMorosidad }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.EliminacionNoPermitida", result.Errors[0].Code);
            Assert.Equal("Eliminación no permitida por estado incorrecto", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_Activa()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_Programada()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerExcepcionesMorosidad()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarExcepcionMorosidad()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Error.Failure());
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTipoTramite()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 0,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task ImportarDatos_ErrorSubTipoTramite()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 0,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoTramiteNTRegimen()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdRegimen = 0,
                    IdTipoAccion = 1,
                    IdTipoTramite = 2,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoTramiteRegimen()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado= 3,
                    IdRegimen = 1,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task ImportarDatos_ErrorTipoIdentificacion()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'X',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.TipoIdentificacionNoEncontrado", result.Errors[0].Code);
            Assert.Equal("Tipo Identificación no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorNumeroIdentificacion()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789012345",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTipoIdentificacionComienza0()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "012345678",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorFechaInicioFechaVencimiento()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2022, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorFechaInicioMenorAlDia()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = new DateTime(2023, 12, 31),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorNombreEmpresa()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa Nombre empresa ",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObservaciones()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones " +
                    "Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones Observaciones ",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCrearExcepcionMorosidad()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now.AddDays(3),
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };
            
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearExcepcionMorosidad_DatosDuplicados()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdTipoAccion = 1,
                IdTipoTramite = 1,
                IdSubtipoTramite = 1,
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            //Act
            CrearExcepcionMorosidadCommand command = new CrearExcepcionMorosidadCommand() { ExcepcionMorosidad = excepcion };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(excepcion, It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(excepcion)).ReturnsAsync(excepcion);
            var handler = new CrearExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarExcepcionMorosidad_DatosDuplicados()
        {
            //Arrange
            var excepcion = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                FechaInicio = DateTime.Now,
                FechaVencimiento = new DateTime(2028, 12, 31),
                IdEstado = 1,
                IdTipoTramite = 1,
                IdSubtipoTramite = 1,
                IdRegimen = 1,
                IdTipoAccion = 1
            };

            var empresa = new Dominio.Entidades.Empresa()
            {
                Id = 1
            };

            var listaCambios = new List<string> { "NumeroIdentificacionEmpresa", "TipoIdentificacionEmpresa", "NombreEmpresa", "IdSubtipoTramite", "IdTipoAccion" };

            //Act
            EditarExcepcionMorosidadCommand command = new EditarExcepcionMorosidadCommand { ExcepcionMorosidad = excepcion, IdExcepcionMorosidad = excepcion.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionMorosidadPorId(1)).ReturnsAsync(excepcion);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>(), It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ActualizarExcepcionMorosidad(command.IdExcepcionMorosidad, command.ListaCambios, command.ExcepcionMorosidad)).ReturnsAsync(excepcion);
            var handler = new EditarExcepcionMorosidadCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_DatosDuplicados()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ExcepcionMorosidad>() 
            { 
                new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = DateTime.Now.AddDays(3),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado= 3,
                    IdRegimen = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1
                }
            };

            var excepcionExistente = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(3),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 3,
                IdRegimen = 1,
                IdTipoTramite = 1,
                IdSubtipoTramite = 1
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>(), It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task ImportarDatos_Validar_DatosDuplicados()
        {
            // Arrange

            var modo = 1;
            var excepcionExistente = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(3),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 3,
                IdTipoAccion = 1,
                IdTipoTramite = 1,
                IdSubtipoTramite = 1,
            };

            List<ExcepcionMorosidad> datos = new List<ExcepcionMorosidad>();
            datos.Add(excepcionExistente);
            datos.Add(excepcionExistente);

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>(), It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ExcepcionMorosidad.DatosDuplicados", result.Errors[0].Code);
            Assert.Equal("Ya existe una excepción de morosidad con los datos proporcionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_DatosDuplicados_Dato()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ExcepcionMorosidad>()
            {
                new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = DateTime.Now.AddDays(3),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado= 3,
                    IdRegimen = 1,
                    IdTipoTramite = ConstantesTipoTramite.TipoTramite_NOTATECNICA,
                    IdSubtipoTramite = 1,
                    IdTipoAccion = 1
                }
            };

            var excepcionExistente = new ExcepcionMorosidad()
            {
                Id = 1,
                TipoIdentificacionEmpresa = 'F',
                NumeroIdentificacionEmpresa = "123456789",
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(3),
                NombreEmpresa = "Nombre empresa",
                Observaciones = "Observaciones",
                IdEstado = 3,
                IdRegimen = 1,
                IdTipoTramite = ConstantesTipoTramite.TipoTramite_NOTATECNICA,
                IdSubtipoTramite = 1,
                IdTipoAccion = 1
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>(), It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_ErrorFalloServicio()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK_DuplicidadRegistroExistente() { 
            // Arrange
            var modo = 1;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ValidarExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>(), It.IsAny<bool>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_Devuelve_ErrorNotaTecnica_RegimenNull()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = ConstantesTipoTramite.TipoTramite_NOTATECNICA,
                    IdSubtipoTramite = 1,
                    IdRegimen = null, // Regimen es null
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_Devuelve_ErrorNotaTecnica_RegimenInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = ConstantesTipoTramite.TipoTramite_REGISTRO,
                    IdSubtipoTramite = 1,
                    IdRegimen = 1
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_Devuelve_ErrorCrearExepcion()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(mockRepo => mockRepo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>()))
                .ReturnsAsync(Error.Failure("Error al crear la excepción de morosidad"));

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_Devuelve_ExisteRelacionTipoTramiteSubtipo_False()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_Devuelve_ExisteRelacionTipoTramiteSubtipo_Error()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ExcepcionMorosidad>() { new ExcepcionMorosidad()
                {
                    Id = 1,
                    TipoIdentificacionEmpresa = 'F',
                    NumeroIdentificacionEmpresa = "123456789",
                    FechaInicio = DateTime.Now,
                    FechaVencimiento = new DateTime(2028, 12, 31),
                    NombreEmpresa = "Nombre empresa",
                    Observaciones = "Observaciones",
                    IdEstado=3,
                    IdTipoAccion = 1,
                    IdTipoTramite = 1,
                    IdSubtipoTramite = 1,
                }
            };

            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.ObtenerExcepcionesMorosidad()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.EliminarExcepcionMorosidad(1)).ReturnsAsync(Result.Deleted);
            mockTramites.Setup(repo => repo.ExisteRelacionTipoTramiteSubtipo(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Error.Failure("fallo"));
            mockRepo.Setup(repo => repo.ExcepcionesMorosidadRepository.CrearExcepcionMorosidad(It.IsAny<ExcepcionMorosidad>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockTramites.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
    }
}
