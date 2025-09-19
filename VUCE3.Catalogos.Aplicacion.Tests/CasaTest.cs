using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasaPorId;
using VUCE3.Catalogos.Aplicacion.Casa.Queries.ObtenerCasas;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EditarCasa;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasa;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.CrearCasa;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.EliminarCasas;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Casa.Commands.ImportarDatos;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CasaTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public CasaTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerCasas_Ok()
        {
            //Act
            var casas = new List<Dominio.Entidades.Casa>()
            {
                new Dominio.Entidades.Casa()
                {
                    Id =1,
                    Codigo = "1",
                    Nombre = "Casa 1"
                }
            };

            ObtenerCasasQuery obtenerCasaQuery = new ObtenerCasasQuery();
            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(casas);
            var handler = new ObtenerCasasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Casa>>(result.Value);
        }

        [Fact]
        public async Task ObtenerCasaPorId_Ok()
        {
            //Arange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            //Act
            ObtenerCasaPorIdQuery obtenerPaisPorIdQuery = new ObtenerCasaPorIdQuery();
            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            var handler = new ObtenerCasaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCasa_Ok()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            var listaCambios = new List<string> { "Nombre" , "Codigo" };

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa= casa, IdCasa= casa.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa( command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(casa);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCasa_Error()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa = casa, IdCasa = casa.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(errorIsError);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarCasa_NoExiste()
        {
            //Arrange

            var Casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresCasa.NoEncontrada;

            //Act

            EditarCasaCommand command = new EditarCasaCommand { Casa= Casa, IdCasa= Casa.Id, ListaCambios= listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(errorIsError);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Casa no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCasa_CodigoError()
        {
            //Arrange

            var Casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "",
                Nombre = "Casa 1"

            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresCasa.NoEncontrada;

            //Act

            EditarCasaCommand command = new EditarCasaCommand { Casa = Casa, IdCasa = Casa.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(Casa);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(errorIsError);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCasa_Ok()
        {
            //Arrange
            var id = 1;

            var Casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            //Act
            EliminarCasaCommand command = new EliminarCasaCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(id)).ReturnsAsync(Casa);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCasa_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresCasa.NoEncontrada;

            //Act
            EliminarCasaCommand command = new EliminarCasaCommand {Id = id};

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Casa no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCasa_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresCasa.NoEncontrada;

            var Casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };

            //Act
            EliminarCasaCommand command = new EliminarCasaCommand { Id = id };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(id)).ReturnsAsync(Casa);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Casa no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCasa_Ok()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            //Act
            
            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(casa);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Casa 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearCasa_ErrorNombre()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = ""

            };

            //Act

            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(casa);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.NombreInvalido", result.FirstError.Code);
            Assert.Equal("El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCasa_ErrorCodigo()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "",
                Nombre = "abc"
            };

            //Act
            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(casa);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.CodigoInvalido", result.FirstError.Code);
            Assert.Equal("El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCasa_Error()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(errorIsError);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(It.IsAny<Dominio.Entidades.Casa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>()
            { 
                new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" },
                new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" }
            };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(It.IsAny<Dominio.Entidades.Casa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ObtenerCasasDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarCasaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(It.IsAny<int>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearCasaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" } };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(It.IsAny<Dominio.Entidades.Casa>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_ErrroExedeValorCampo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() {
                new Dominio.Entidades.Casa() 
                {
                    Codigo = "1",
                    Nombre = "AERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDAERTYGVBSDF"
                }
            };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(It.IsAny<Dominio.Entidades.Casa>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminarCasas_DevuelveOk()
        {
            // Arrange
            var idsCasas = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarCasasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCasasCommand { IdsCasas = idsCasas }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCasas_DevuelveError()
        {
            // Arrange
            var idsCasas = new List<int> { 1, 2, 3 };
            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CasaRepository.EliminarCasa(3)).ReturnsAsync(erroror);

            var handler = new EliminarCasasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCasasCommand { IdsCasas = idsCasas }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarCasas_CrearDevuelveError()
        {
            // Arrange

            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"

            };

            //Act           
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(casa.Id, casa.Codigo,casa.Nombre)).ReturnsAsync(true);
            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(casa);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una casa con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCasas_EditarDevuelveExiste()
        {
            //Arrange

            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa = casa, IdCasa = casa.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(casa.Id, casa.Codigo, casa.Nombre)).ReturnsAsync(true);            
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(casa);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Casa.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una casa con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCasas_EditarDevuelveError()
        {
            //Arrange

            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Casa 1"
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa = casa, IdCasa = casa.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(casa.Id, casa.Codigo, casa.Nombre)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(casa);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarCasasDevuelveOk()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "A" } };            

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.Equal("Casa.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una casa con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCasa_DevuelveValidacionCodigoError()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "111111111111111111111",
                Nombre = "Casa 1"

            };

            var error = ErroresCasa.CasaCodigoInvalido;

            //Act

            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(error);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearCasa_DevuelveValidacionNombreError()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1111",
                Nombre = "Casa 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"

            };

            var error = ErroresCasa.CasaNombreInvalido;

            //Act

            CrearCasaCommand command = new CrearCasaCommand() { Casa = casa };
            mockRepo.Setup(repo => repo.CasaRepository.CrearCasa(casa)).ReturnsAsync(error);

            var handler = new CrearCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }


        [Fact]
        public async Task ActualizarCasa_DevuelveValidacionCodigoError()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "111111111111111111111",
                Nombre = "Casa 1"
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresCasa.CasaCodigoInvalido;

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa = casa, IdCasa = casa.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(error);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCasa_DevuelveValidacionNombreError()
        {
            //Arrange
            var casa = new Dominio.Entidades.Casa()
            {
                Id = 1,
                Codigo = "1111",
                Nombre = "Casa 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresCasa.CasaNombreInvalido;

            //Act
            EditarCasaCommand command = new EditarCasaCommand { Casa = casa, IdCasa = casa.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasaPorId(1)).ReturnsAsync(casa);
            mockRepo.Setup(repo => repo.CasaRepository.ActualizarCasa(command.IdCasa, command.ListaCambios, command.Casa)).ReturnsAsync(error);
            var handler = new EditarCasaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCodigoError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "2222222222222222222222222222222222222222222222", Nombre = "Casa" } };

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(true);

            var error = ErroresCasa.CasaCodigoInvalido;

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Casa>() { new Dominio.Entidades.Casa() { Codigo = "1", Nombre = "Casa 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" } };

            var error = ErroresCasa.CasaNombreInvalido;

            mockRepo.Setup(repo => repo.CasaRepository.ObtenerCasas()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CasaRepository.ValidarCasa(datos[0].Id, datos[0].Codigo, datos[0].Nombre)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

    }
}
