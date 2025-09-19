using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvincias;
using VUCE3.Catalogos.Aplicacion.Provincia.Queries.ObtenerProvinciaPorId;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EditarProvincia;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.EliminarProvincia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Provincia.Commands.CrearProvincia;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.EliminarProvincias;
using VUCE3.Catalogos.Aplicacion.Provincias.Commands.ImportarDatos;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ProvinciaTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public ProvinciaTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerProvincias_Ok()
        {
            //Act
            var Provincias = new List<Dominio.Entidades.Provincia>()
            {
                new Dominio.Entidades.Provincia()
                {
                    Id =1,
                    Nombre = "Heredia",
                    Codigo = "1",
                    Cantones = new List<Dominio.Entidades.Canton>()
                }
            };

            ObtenerProvinciasQuery obtenerCasaQuery = new ObtenerProvinciasQuery();
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(Provincias);
            var handler = new ObtenerProvinciasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Provincia>>(result.Value);
        }

        [Fact]
        public async Task ObtenerProvinciaPorId_Ok()
        {
            //Arange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Heredia",
                Codigo = "1"
            };

            //Act
            ObtenerProvinciaPorIdQuery obtenerProvinciasPorIdQuery = new ObtenerProvinciaPorIdQuery();
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            var handler = new ObtenerProvinciaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProvinciasPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProvincia_Ok()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Heredia",
                Codigo = "1"
            };

            var listaCambios = new List<string> { "Codigo" };

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(Provincia);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProvincia_Error()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"

            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = 1, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(errorIsError);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarProvincia_NoExiste()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"

            };
            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresProvincia.NoEncontrada;

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(errorIsError);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Provincia no encontrada", result.FirstError.Description);
        }
        
        [Fact]
        public async Task ActualizarProvincia_ErrorNombreInvalido()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "".PadRight(51, 'p'),
                Codigo = "1"
            };

            var listaCambios = new List<string> { "Codigo", "Nombre"};

            var error = ErroresProvincia.ProvinciaNombreInvalido;

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(error);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
            Assert.Equal("Provincia.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarProvincia_ErrorCodigoInvalido()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Nombre",
                Codigo = "10"
            };

            var listaCambios = new List<string> { "Codigo" };

            var error = ErroresProvincia.ProvinciaCodigoInvalido;

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(error);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
            Assert.Equal("Provincia.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 1 caracter", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProvincia_Ok()
        {
            //Arrange
            var id = 1;

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"

            };

            var lstCantones = new List<Dominio.Entidades.Canton>();

            //Act
            EliminarProvinciaCommand command = new EliminarProvinciaCommand();
            command.Id = id;

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(id)).ReturnsAsync(Provincia);
         
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProvincia_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresProvincia.NoEncontrada;

            //Act
            EliminarProvinciaCommand command = new EliminarProvinciaCommand { Id = id };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Provincia no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProvincia_ErrorCantonesRelacionados()
        {
            //Arrange
            var id = 1;

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1",
                
            };

            var lstCantones = new List<Dominio.Entidades.Canton>()
                { new Dominio.Entidades.Canton()
                    {
                        Id = 1,
                        Codigo = "01",
                        Nombre = "Nombre 1",
                        IdProvincia = 1
                    }
                };

            //Act
            EliminarProvinciaCommand command = new EliminarProvinciaCommand();
            command.Id = id;

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(id)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.CantonesRelacionados", result.FirstError.Code);
            Assert.Equal("La(s) provincia(s) tiene(n) cantones relacionados", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProvincia_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresProvincia.NoEncontrada;

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"
            };
            var lstCantones = new List<Dominio.Entidades.Canton>();

            //Act
            EliminarProvinciaCommand command = new EliminarProvinciaCommand { Id = id };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(id)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Provincia no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProvincia_Ok()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"
            };

            //Act            
            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(Provincia);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Provincia 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearProvincia_Error()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(errorIsError);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarProvincia_CrearDevuelveError()
        {
            // Arrange

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"

            };

            //Act           
            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(Provincia.Id, Provincia.Codigo)).ReturnsAsync(true);
            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(Provincia);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una provincia con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProvincia_ErrorNombreInvalido()
        {
            // Arrange

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "".PadRight(51, 'p'),
                Codigo = "1"

            };

            //Act           
            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(Provincia.Id, Provincia.Codigo)).ReturnsAsync(true);
            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(Provincia);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProvincia_ErrorCodigoInvalido()
        {
            // Arrange

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "10"

            };

            //Act           
            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(Provincia.Id, Provincia.Codigo)).ReturnsAsync(true);
            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(Provincia);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 1 caracter", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarProvincia_EditarDevuelveExiste()
        {
            //Arrange

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"
            };

            var listaCambios = new List<string> { "Codigo", "Nombre" };

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(Provincia.Id, Provincia.Codigo)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(Provincia);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una provincia con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarProvincia_EditarDevuelveError()
        {
            //Arrange

            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "Provincia 1",
                Codigo = "1"
            };


            var listaCambios = new List<string> { "Codigo", "Nombre" };

            //Act
            EditarProvinciaCommand command = new EditarProvinciaCommand { Provincia = Provincia, IdProvincia = Provincia.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Provincia);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(Provincia.Id, Provincia.Codigo)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProvinciaRepository.ActualizarProvincia(command.IdProvincia, command.ListaCambios, command.Provincia)).ReturnsAsync(Provincia);
            var handler = new EditarProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProvincia_DevuelveValidacionNombreError()
        {
            //Arrange
            var Provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Nombre = "".PadRight(51, 'a'),
                Codigo = "1"

            };

            var error = ErroresProvincia.ProvinciaNombreInvalido;

            //Act

            CrearProvinciaCommand command = new CrearProvinciaCommand() { Provincia = Provincia };
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(Provincia)).ReturnsAsync(error);

            var handler = new CrearProvinciaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        
        [Fact]
        public async Task EliminadoMasivoProvincia_DevuelveOk()
        {
            // Arrange
            var idsProvincias = new List<int> { 1, 2 };

            var lstCantones = new List<Dominio.Entidades.Canton>();

            var provincia1 = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1"
            };

            var provincia2 = new Dominio.Entidades.Provincia()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2"
            };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia1);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(2)).ReturnsAsync(provincia2);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarProvinciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProvinciasCommand { IdsProvincias = idsProvincias }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoProvincia_ErrorObtenerProvinciaPorId()
        {
            // Arrange
            var idsProvincias = new List<int> { 1, 2 };

            var provincia1 = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1"
            };

            var provincia2 = new Dominio.Entidades.Provincia()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2"
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(2)).ReturnsAsync(provincia2);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarProvinciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProvinciasCommand { IdsProvincias = idsProvincias }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoProvincia_ErrorEliminar()
        {
            // Arrange
            var idsProvincias = new List<int> { 1, 2 };

            var provincia1 = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1"
            };

            var provincia2 = new Dominio.Entidades.Provincia()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2"
            };

            var lstCantones = new List<Dominio.Entidades.Canton>();

            var erroror = ErroresProvincia.NoEncontrada;

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia1);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(2)).ReturnsAsync(provincia2);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(2)).ReturnsAsync(erroror);

            var handler = new EliminarProvinciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProvinciasCommand { IdsProvincias = idsProvincias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.NoEncontrada", result.Errors[0].Code);
            Assert.Equal("Provincia no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminadoMasivoProvincia_ErrorCantonesRelacionados()
        {
            // Arrange
            var idsProvincias = new List<int> { 1, 2 };

            var provincia1 = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1"
            };

            var provincia2 = new Dominio.Entidades.Provincia()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2"
            };

            var lstCantones = new List<Dominio.Entidades.Canton>() { new Dominio.Entidades.Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdProvincia =1
                }
            };

            var erroror = ErroresProvincia.ProvinciaCantonesRelacionados;

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia1);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(2)).ReturnsAsync(provincia2);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(2)).ReturnsAsync(erroror);

            var handler = new EliminarProvinciasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProvinciasCommand { IdsProvincias = idsProvincias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.CantonesRelacionados", result.Errors[0].Code);
            Assert.Equal("La(s) provincia(s) tiene(n) cantones relacionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ExisteRelacionConCanton(1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveErroValidadndoCanton()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ExisteRelacionConCanton(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveRelacionCanton_Ok()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ExisteRelacionConCanton(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveExistenDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() {
                new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                },
                new Dominio.Entidades.Provincia()
                {
                    Id = 2,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorModo()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ValidarProvincia(1, "1")).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

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
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre =  "".PadRight(51, 'a')
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CodigoInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre =  "Nombre"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Provincia.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 1 caracter", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerProvincia()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarProvincia()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(datos[0]);

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
            var datos = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.EliminarProvincia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProvinciaRepository.CrearProvincia(It.IsAny<Dominio.Entidades.Provincia>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
                
    }
}
