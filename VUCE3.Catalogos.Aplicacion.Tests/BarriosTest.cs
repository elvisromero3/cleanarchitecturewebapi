using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.CrearBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EditarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrio;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.EliminarBarrios;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Barrios.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrioPorId;
using VUCE3.Catalogos.Aplicacion.Barrios.Queries.ObtenerBarrios;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class BarriosTest
    {
        private Mock<IUnitOfWork> mockRepo;

        public BarriosTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task ObtenerBarrios_Ok()
        {
            //Arange
            var lstDistritos = new List<Barrio>()
            {
                new Barrio{
                    Id = 1,
                    Codigo = "COD1",
                    Nombre = "Barrio 1",
                    IdDistrito = 1
                },
                new Barrio{
                    Id = 2,
                    Codigo = "COD2",
                    Nombre = "Barrio 2",
                    IdDistrito = 1
                },
            };
            //Act
            ObtenerBarriosQuery obtenerBarriosQuery = new ObtenerBarriosQuery();
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(lstDistritos);
            var handler = new ObtenerBarriosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerBarriosQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ObtenerBarrioPorId_Ok()
        {
            //Arange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            ObtenerBarrioPorIdQuery obtenerBarrioPorIdQuery = new ObtenerBarrioPorIdQuery();
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            var handler = new ObtenerBarrioPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerBarrioPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearBarrio_Ok()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(barrio);
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Barrio 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearBarrio_ErrorNombre()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(barrio);
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearBarrio_ErrorCodigo()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "10000000",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(barrio);
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 7 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearBarrio_ErrorDistritoNoExiste()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(ErroresDistrito.DistritoNoEncontrado);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(barrio);
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Distrito.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Distrito no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearBarrio_Duplicado()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(barrio);
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un barrio con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearBarrio_ErrorCrear()
        {
            //Arrange
            var distrito = new Distrito
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Distrito 1",
                IdCanton = 1
            };
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Barrio 1",
                IdDistrito = 1
            };

            //Act
            CrearBarrioCommand command = new CrearBarrioCommand() { Barrio = barrio };
            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritoPorId(barrio.IdDistrito)).ReturnsAsync(distrito);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, barrio.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(barrio)).ReturnsAsync(Error.Failure());
            var handler = new CrearBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_Ok()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarBarrio_BarrioNoExiste()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(ErroresBarrio.BarrioNoEncontrado);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Barrio no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_DistritoNoActualizable()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "IdDistrito", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.DistritoNoActualizable", result.FirstError.Code);
            Assert.Equal("El campo distrito no es actualizable", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_ErrorNombre()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_ErrorCodigoInvalido()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "10000000",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 7 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_ErrorValidar()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, command.Barrio.Codigo, 0)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_DatosDuplicados()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ValidarBarrio(1, command.Barrio.Codigo, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(barrio);
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un barrio con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarBarrio_ErrorActualizar()
        {
            //Arrange
            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            var listaCambios = new List<string> { "Nombre", "Codigo" };

            //Act
            EditarBarrioCommand command = new EditarBarrioCommand() { Barrio = barrio, IdBarrio = barrio.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.ActualizarBarrio(command.Barrio, command.IdBarrio, command.ListaCambios)).ReturnsAsync(Error.Failure());
            var handler = new EditarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarBarrio_Ok()
        {
            //Arrange
            var id = 1;

            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            //Act
            EliminarBarrioCommand command = new EliminarBarrioCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(id)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarBarrio_BarrioNoExiste()
        {
            //Arrange
            var id = 1;

            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            //Act
            EliminarBarrioCommand command = new EliminarBarrioCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(id)).ReturnsAsync(ErroresBarrio.BarrioNoEncontrado);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Barrio no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarBarrio_ErrorEliminar()
        {
            //Arrange
            var id = 1;

            var barrio = new Barrio
            {
                Id = 1,
                Codigo = "cod",
                Nombre = "Nombre",
                IdDistrito = 1
            };

            //Act
            EliminarBarrioCommand command = new EliminarBarrioCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(id)).ReturnsAsync(barrio);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(id)).ReturnsAsync(Error.Failure());
            var handler = new EliminarBarrioCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminadoMasivoBarrio_DevuelveOk()
        {
            // Arrange
            var idsBarrios = new List<int> { 1, 2 };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var lstBarrios = new List<Barrio>();
            lstBarrios.Add(barrio1);
            lstBarrios.Add(barrio2);

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(lstBarrios);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio1);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(2)).ReturnsAsync(barrio2);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarBarriosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBarriosCommand { IdsBarrios = idsBarrios }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoBarrio_ErrorObtenerBarrioPorId()
        {
            // Arrange
            var idsBarrios = new List<int> { 1, 2 };
            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var lstBarrios = new List<Barrio>();
            lstBarrios.Add(barrio2);

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(lstBarrios);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(2)).ReturnsAsync(barrio2);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarBarriosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBarriosCommand { IdsBarrios = idsBarrios }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoBarrio_ErrorEliminar()
        {
            // Arrange
            var idsBarrios = new List<int> { 1, 2 };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };
            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var lstBarrios = new List<Barrio>();
            lstBarrios.Add(barrio1);
            lstBarrios.Add(barrio2);

            var erroror = ErroresBarrio.BarrioNoEncontrado;

            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(lstBarrios);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(1)).ReturnsAsync(barrio1);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrioPorId(2)).ReturnsAsync(barrio2);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(2)).ReturnsAsync(erroror);

            var handler = new EliminarBarriosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBarriosCommand { IdsBarrios = idsBarrios }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Barrio no encontrado", result.Errors[0].Description);
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

            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

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
            var datos = new List<ImportarBarrioCommandDto>() {
                new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
                },
                new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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
            var datos = new List<ImportarBarrioCommandDto>() {
                new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
                },
                new ImportarBarrioCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

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
            var datos = new List<ImportarBarrioCommandDto>() {
                new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 3,
                    Distrito = 1
                },
                new ImportarBarrioCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorDistritoInexistente()
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
            var datos = new List<ImportarBarrioCommandDto>() {
                new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 3
                },
                new ImportarBarrioCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

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
            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre =  "".PadRight(101, 'a'),
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_CodigoInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "10000001",
                    Nombre =  "Nombre",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Barrio.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 7 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerBarrios()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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
            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

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
            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
                }
            };
            
            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
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
            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(barrios[0]);

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
            var datos = new List<ImportarBarrioCommandDto>() { new ImportarBarrioCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1,
                    Canton = 1,
                    Distrito = 1
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
            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton = 1
                }
            };

            var barrio1 = new Barrio()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdDistrito = 1
            };

            var barrio2 = new Barrio()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdDistrito = 1
            };

            var barrios = new List<Barrio>();
            barrios.Add(barrio1);
            barrios.Add(barrio2);

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.BarriosRepository.ObtenerBarrios()).ReturnsAsync(barrios);
            mockRepo.Setup(repo => repo.BarriosRepository.EliminarBarrio(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BarriosRepository.CrearBarrio(It.IsAny<Barrio>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
