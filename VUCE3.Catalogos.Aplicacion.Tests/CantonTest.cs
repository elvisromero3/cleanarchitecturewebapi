using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantones;
using VUCE3.Catalogos.Aplicacion.Cantones.Queries.ObtenerCantonPorId;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EditarCanton;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCanton;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.CrearCanton;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.EliminarCantones;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.Cantones.Commands.ImportarDatos.DTO;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CantonTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        public CantonTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerCantons_Ok()
        {
            //Act
            var Cantons = new List<Canton>()
            {
                new Canton()
                {
                    Id =1,
                    Codigo= "1",
                    Nombre= "Canton 1",
                    IdProvincia = 1,
                    Provincia = new Dominio.Entidades.Provincia()
                }
            };

            ObtenerCantonesQuery obtenerCasaQuery = new ObtenerCantonesQuery();
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(Cantons);
            var handler = new ObtenerCantonesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Canton>>(result.Value);
        }

        [Fact]
        public async Task ObtenerCantonPorId_Ok()
        {
            //Arange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };

            //Act
            ObtenerCantonPorIdQuery obtenerCantonsPorIdQuery = new ObtenerCantonPorIdQuery();
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            var handler = new ObtenerCantonPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCantonsPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCanton_Ok()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };

            var listaCambios = new List<string> { "Codigo" };

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(Canton);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCanton_Error()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1

            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = Error.Unexpected();

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = 1, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(errorIsError);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarCanton_NoExiste()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1

            };
            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresCanton.NoEncontrado;

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(errorIsError);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cantón no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCanton_Ok()
        {
            //Arrange
            var id = 1;

            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1

            };

            //Act
            EliminarCantonCommand command = new EliminarCantonCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(id)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCanton_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresCanton.NoEncontrado;

            //Act
            EliminarCantonCommand command = new EliminarCantonCommand { Id = id };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cantón no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCanton_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresCanton.NoEncontrado;

            var Canton = new Dominio.Entidades.Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };

            //Act
            EliminarCantonCommand command = new EliminarCantonCommand { Id = id };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(id)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Cantón no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCanton_Ok()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1

            };

            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            
            //Act            
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(Canton);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Canton 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearCanton_Error()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1

            };
            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };
            var errorIsError = Error.Unexpected();

            //Act
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(errorIsError);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarCanton_CrearDevuelveError()
        {
            // Arrange

            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };
            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            //Act
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(true);
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(Canton);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un cantón con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCanton_ErrorProvinciaInexistente()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "Nombre",
                Codigo = "1",
                IdProvincia = 1
            };

            //Act            
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(ErroresCanton.ProvinciaInexistente);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(false);
            //mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(error);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Canton.ProvinciaInexistente", result.FirstError.Code);
            Assert.Equal("Provincia inexistente", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCanton_DevuelveValidacionNombreError()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "".PadRight(101, 'a'),
                Codigo = "1",
                IdProvincia = 1
            };

            var error = ErroresCanton.NombreInvalido;
            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            //Act            
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(error);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearCanton_DevuelveValidacionCodigoError()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "Nombre",
                Codigo = "1000",
                IdProvincia = 1
            };

            var error = ErroresCanton.CantonCodigoInvalido;
            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            //Act            
            CrearCantonCommand command = new CrearCantonCommand() { Canton = Canton };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(Canton)).ReturnsAsync(error);

            var handler = new CrearCantonesCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);;
            Assert.Equal("Canton.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 3 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCantones_EditarDevuelveExiste()
        {
            //Arrange

            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };


            var listaCambios = new List<string> { "Codigo", "Nombre" };

            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            //Act
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(Canton);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un cantón con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCanton_EditarDevuelveError()
        {
            //Arrange

            var Canton = new Canton()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Canton 1",
                IdProvincia = 1
            };
            var listaCambios = new List<string> { "Codigo", "Nombre" };

            var provincia = new Dominio.Entidades.Provincia()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
            };

            //Act            
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvinciaPorId(1)).ReturnsAsync(provincia);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ValidarCantones(Canton.Id, Canton.Codigo, 0)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(Canton);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }
        
        [Fact]
        public async Task ActualizarCanton_DevuelveValidacionNombreError()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "".PadRight(51, 'a'),
                Codigo = "1"
            };

            var listaCambios = new List<string> { "Codigo", "Nombre" };

            var error = ErroresCanton.NombreInvalido;

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(error);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCanton_DevuelveValidacionCodigoError()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "Nombre",
                Codigo = "1000"
            };

            var listaCambios = new List<string> { "Codigo", "Nombre" };

            var error = ErroresCanton.CantonCodigoInvalido;

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(error);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCanton_ErrorProvinciaNoActualizable()
        {
            //Arrange
            var Canton = new Canton()
            {
                Id = 1,
                Nombre = "Nombre",
                Codigo = "100",
                IdProvincia = 2
            };

            var listaCambios = new List<string> { "IdProvincia" };

            var error = ErroresCanton.CantonProvinciaNoActualizable;

            //Act
            EditarCantonCommand command = new EditarCantonCommand { Canton = Canton, IdCanton = Canton.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Canton);
            mockRepo.Setup(repo => repo.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton)).ReturnsAsync(error);
            var handler = new EditarCantonCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task EliminadoMasivoCanton_DevuelveOk()
        {
            // Arrange
            var idsCantones = new List<int> { 1, 2 };

            var lstDistritos = new List<Distrito>();

            var canton1 = new Canton()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdProvincia = 1
            };

            var canton2 = new Canton()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdProvincia = 1
            };

            var lstCantones = new List<Canton>();
            lstCantones.Add(canton1);
            lstCantones.Add(canton2);

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(canton1);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(2)).ReturnsAsync(canton2);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarCantonesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCantonesCommand { IdsCantones = idsCantones }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoCanton_ErrorObtenerCantonPorId()
        {
            // Arrange
            var idsCantones = new List<int> { 1, 2 };

            var canton1 = new Canton()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdProvincia = 1
            };

            var canton2 = new Canton()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdProvincia = 1
            };
            var lstCantones = new List<Canton>();
            lstCantones.Add(canton2);

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(2)).ReturnsAsync(canton2);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarCantonesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCantonesCommand { IdsCantones = idsCantones }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoCanton_ErrorDistritosRelacionados()
        {
            // Arrange
            var idsCantones = new List<int> { 1, 2 };
            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton =1
                }
            };

            var canton1 = new Canton()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdProvincia = 1,
                Distritos = distritos
            };

            var canton2 = new Canton()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdProvincia = 1
            };

            var lstDistritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton =1
                }
            };

            var lstCantones = new List<Canton>();
            lstCantones.Add(canton1);
            lstCantones.Add(canton2);
                       
            var erroror = ErroresCanton.DistritosRelacionados;
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);           
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(canton1);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(2)).ReturnsAsync(canton2);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(2)).ReturnsAsync(erroror);

            var handler = new EliminarCantonesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCantonesCommand { IdsCantones = idsCantones }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.DistritosRelacionados", result.Errors[0].Code);
            Assert.Equal("Existe cantón con distritos relacionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminadoMasivoCanton_ErrorEliminar()
        {
            // Arrange
            var idsCantones = new List<int> { 1, 2 };

            var canton1 = new Canton()
            {
                Id = 1,
                Codigo = "01",
                Nombre = "Nombre 1",
                IdProvincia = 1
            };

            var canton2 = new Canton()
            {
                Id = 2,
                Codigo = "02",
                Nombre = "Nombre 2",
                IdProvincia = 1
            };
            var lstCantones = new List<Canton>();
            lstCantones.Add(canton1);
            lstCantones.Add(canton2);

            var erroror = ErroresCanton.NoEncontrado;

            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(lstCantones);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(1)).ReturnsAsync(canton1);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantonesPorId(2)).ReturnsAsync(canton2);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(2)).ReturnsAsync(erroror);

            var handler = new EliminarCantonesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCantonesCommand { IdsCantones = idsCantones }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Cantón no encontrado", result.Errors[0].Description);
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

            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
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
           
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

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
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia",
                }
            };
            var datos = new List<ImportarCantonCommandDto>() {
                new ImportarCantonCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
                },
                new ImportarCantonCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

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
            var datos = new List<ImportarCantonCommandDto>() {
                new ImportarCantonCommandDto()
                {
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
                },
                new ImportarCantonCommandDto()
                {
                    Codigo = "02",
                    Nombre = "Nombre 2",
                    Provincia = 1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

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
            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {                   
                    Codigo = "01",
                    Nombre =  "".PadRight(51, 'a'),
                    Provincia =1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.NombreInvalido", result.Errors[0].Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_CodigoInvalido()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {
                    Codigo = "0100",
                    Nombre =  "Nombre",
                    Provincia =1
                }
            };
            var provincias = new List<Dominio.Entidades.Provincia>() { new Dominio.Entidades.Provincia()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Provincia"
                }
            };
            var cantones = new List<Canton>() { new Canton()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Canton.CodigoInvalido", result.Errors[0].Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 3 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerCantones()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {                    
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarCanton()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {                    
                    Codigo = "01",
                    Nombre = "Nombre",
                    Provincia = 1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(cantones[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(!result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_ErrorDistritosRelacionados()
        {
            // Arrange
            var modo = 2;

            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {                    
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
                }
            };

            var distritos = new List<Distrito>() { new Distrito()
                {
                    Id = 1,
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    IdCanton =1
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

            mockRepo.Setup(repo => repo.DistritosRepository.ObtenerDistritos()).ReturnsAsync(distritos);
            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);            
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(ErroresCanton.DistritosRelacionados);
            
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(!result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCrear()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCantonCommandDto>() { new ImportarCantonCommandDto()
                {                   
                    Codigo = "01",
                    Nombre = "Nombre 1",
                    Provincia = 1
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

            mockRepo.Setup(repo => repo.ProvinciaRepository.ObtenerProvincias()).ReturnsAsync(provincias);
            mockRepo.Setup(repo => repo.CantonesRepository.ObtenerCantones()).ReturnsAsync(cantones);
            mockRepo.Setup(repo => repo.CantonesRepository.EliminarCantones(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CantonesRepository.CrearCantones(It.IsAny<Canton>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
