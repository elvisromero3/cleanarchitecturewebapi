using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.CrearDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EditarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistrito;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.EliminarDistritos;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Distritos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritoPorId;
using VUCE3.Catalogos.Aplicacion.Distritos.Queries.ObtenerDistritos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class DistritosTest
    {
        private Mock<IUnitOfWork> mockRepo;

        public DistritosTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerDistritos_Ok()
        {
            //Arange
            var lstDistritos = new List<Distrito>()
            {
                new Distrito{
                    Id = 1,
                    Codigo = "COD",
                    Nombre = "Distrito 1",
                    IdCanton = 1
                },
                new Distrito{
                    Id = 2,
                    Codigo = "COD",
                    Nombre = "Distrito 2",
                    IdCanton = 1
                },
            };

            //Act
            ObtenerDistritosQuery obtenerDistritosQuery = new ObtenerDistritosQuery();
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(lstDistritos);
            var handler = new ObtenerDistritosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerDistritosQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ObtenerDistritoPorId_Ok()
        {
            //Arange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Act
            ObtenerDistritoPorIdQuery obtenerDistritoPorIdQuery = new ObtenerDistritoPorIdQuery();
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            var handler = new ObtenerDistritoPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerDistritoPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearDistrito_Ok()
        {
            //Arrange
            var canton = new Canton
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdProvincia = 1
            };

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(distrito.IdCanton)).ReturnsAsync(canton);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(distrito);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Distrito 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearDistrito_ErrorNombre()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "",
                IdCanton = 1
            };

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(distrito);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Distrito.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearDistrito_ErrorCodigo()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "100000",
                Nombre = "Nombre",
                IdCanton = 1
            };

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(distrito);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Distrito.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 5 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearDistrito_CantonNoExiste()
        {
            //Arrange
            var canton = new Canton
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdProvincia = 1
            };

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(distrito.IdCanton)).ReturnsAsync(ErroresCanton.NoEncontrado);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(distrito);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cantón no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearDistrito_Error()
        {
            //Arrange
            var canton = new Canton
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdProvincia = 1
            };

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var error = Error.Unexpected();

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(distrito.IdCanton)).ReturnsAsync(canton);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(error);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.FirstError.Code);
            Assert.Equal("An unexpected error has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearDistrito_DatosDuplicados()
        {
            //Arrange
            var canton = new Canton
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdProvincia = 1
            };
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var error = ErroresDistrito.DistritoDatosDuplicados;

            //Act
            CrearDistritoCommand command = new CrearDistritoCommand() { Distrito = distrito };
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(distrito.IdCanton)).ReturnsAsync(canton);
            mockRepo.Setup(repo => repo.DistritosRepository.ValidarDistrito(command.Distrito.Id, command.Distrito.Codigo, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(distrito)).ReturnsAsync(ErroresDistrito.DistritoDatosDuplicados);
            var handler = new CrearDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un distrito con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_Ok()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarDistrito_NoExiste()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresDistrito.DistritoNoEncontrado;

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Distrito no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_CantonNoActualizable()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "IdCanton" };
            var errorIsError = ErroresDistrito.DistritoNoEncontrado;

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.CantonNoActualizable", result.FirstError.Code);
            Assert.Equal("El campo cantón no es actualizable", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_ErrorNombre()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresDistrito.DistritoNombreInvalido;

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_ErrorCodigo()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "100000",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresDistrito.DistritoCodigoInvalido;

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 5 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_Error()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(errorIsError);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarDistrito_ValidarDuplicado()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresDistrito.DistritoDatosDuplicados;

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ValidarDistrito(distrito.Id, distrito.Codigo, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un distrito con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarDistrito_ErrorValidar()
        {
            //Arrange

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            var listaCambios = new List<string> { "Codigo" };

            //Act

            EditarDistritoCommand command = new EditarDistritoCommand() { Distrito = distrito, IdDistrito = distrito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.ValidarDistrito(distrito.Id, distrito.Codigo, 0)).ReturnsAsync(Error.Unexpected());
            mockRepo.Setup(repo => repo.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios)).ReturnsAsync(distrito);
            var handler = new EditarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarDistrito_Ok()
        {
            //Arrange
            var id = 1;

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            //Act
            EliminarDistritoCommand command = new EliminarDistritoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(id)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarDistrito_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresDistrito.DistritoNoEncontrado;

            //Act
            EliminarDistritoCommand command = new EliminarDistritoCommand { Id = id };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Distrito no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarDistrito_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresDistrito.DistritoNoEncontrado;

            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdCanton = 1
            };

            //Act
            EliminarDistritoCommand command = new EliminarDistritoCommand { Id = id };

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(id)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarDistritoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Distrito no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminadoMasivoDistrito_DevuelveOk()
        {
            // Arrange
            var idsDistritos = new List<int> { 1, 2 };

            var lstBarrios = new List<Barrio>();

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var lstDistritos = new List<Distrito>();
            lstDistritos.Add(distrito1);
            lstDistritos.Add(distrito2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(lstDistritos);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito1);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(2)).ReturnsAsync(distrito2);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarDistritosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarDistritosCommand { IdsDistritos = idsDistritos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoDistrito_ErrorObtenerDistritoPorId()
        {
            // Arrange
            var idsDistritos = new List<int> { 1, 2 };
            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var lstDistritos = new List<Distrito>();
            lstDistritos.Add(distrito2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(lstDistritos);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(2)).ReturnsAsync(distrito2);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarDistritosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarDistritosCommand { IdsDistritos = idsDistritos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoDistrito_ErrorBarriosRelacionados()
        {
            // Arrange
            var idsDistritos = new List<int> { 1, 2 };
            var barrios = new List<Barrio>() { new Barrio()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdDistrito = 1
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1,
                Barrios = barrios
            };
            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var lstDistritos = new List<Distrito>();
            lstDistritos.Add(distrito1);
            lstDistritos.Add(distrito2);

            var erroror = ErroresDistrito.BarriosRelacionados;

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(lstDistritos);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito1);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(2)).ReturnsAsync(distrito2);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(2)).ReturnsAsync(erroror);

            var handler = new EliminarDistritosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarDistritosCommand { IdsDistritos = idsDistritos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.BarriosRelacionados", result.Errors[0].Code);
            Assert.Equal("Existe distrito con barrios relacionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminadoMasivoDistrito_ErrorEliminar()
        {
            // Arrange
            var idsDistritos = new List<int> { 1, 2 };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };
            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var lstDistritos = new List<Distrito>();
            lstDistritos.Add(distrito1);
            lstDistritos.Add(distrito2);

            var erroror = ErroresDistrito.DistritoNoEncontrado;

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(lstDistritos);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(1)).ReturnsAsync(distrito1);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(2)).ReturnsAsync(distrito2);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(2)).ReturnsAsync(erroror);

            var handler = new EliminarDistritosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarDistritosCommand { IdsDistritos = idsDistritos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Distrito no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 1;

            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };

            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdProvincia = 1
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveExistenDuplicadosPorCodigo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() {
                new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                },
                new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                }
            };

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorProvinciaInexistente()
        {
            // Arrange
            var modo = 2;
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 2,
                    Codigo = "01",
                    Nombre = "Provincia 2",
                }
            };
            var datos = new List<ImportarDistritoCommandDto>() {
                new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                },
                new ImportarDistritoCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var cantones = new List<Canton>() {
                new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdProvincia = 1
                },
                new Canton()
                {
                    Id = 2,
                    Codigo = "02",
                    Nombre = "Nombre 1",
                    IdProvincia = 1
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCantonInexistente()
        {
            // Arrange
            var modo = 2;
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var datos = new List<ImportarDistritoCommandDto>() {
                new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 3

                },
                new ImportarDistritoCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var cantones = new List<Canton>() {
                new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdProvincia = 1
                },
                new Canton()
                {
                    Id = 2,
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    IdProvincia = 1
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

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
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre =  "".PadRight(51, 'a'),
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_CodigoInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "1000000",
                    Nombre =  "Nombre",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 5 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerDistritos()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };
            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarDistrito()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);



            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync((ErrorOr<List<Canton>>)cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync((ErrorOr<List<Distrito>>)distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync((ErrorOr<List<Dominio.Entidades.Provincia>>)provincias);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(distritos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorBarriosRelacionados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };

            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(ErroresDistrito.BarriosRelacionados);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCrear()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarDistritoCommandDto>() { new ImportarDistritoCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };
            var distrito1 = new Distrito()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdCanton = 1
            };

            var distrito2 = new Distrito()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdCanton = 1
            };

            var distritos = new List<Distrito>();
            distritos.Add(distrito1);
            distritos.Add(distrito2);

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.DistritosRepository.EliminarDistrito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.DistritosRepository.CrearDistrito(It.IsAny<Distrito>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
