using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamiliasPorId;
using VUCE3.Catalogos.Aplicacion.Familia.Queries.ObtenerFamilias;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EditarFamilia;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.CrearFamilia;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class FamiliaTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public FamiliaTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerFamilias_Ok()
        {
            //Act
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia()
                {
                    Id =1,
                    Nombre = "Familia 1"
                }
            };

            ObtenerFamiliasQuery obtenerFamiliaQuery = new ObtenerFamiliasQuery();
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            var handler = new ObtenerFamiliasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerFamiliaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Familia>>(result.Value);
        }

        [Fact]
        public async Task ObtenerFamiliaPorId_Ok()
        {
            //Arange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            //Act
            ObtenerFamiliaPorIdQuery obtenerFamiliaPorIdQuery = new ObtenerFamiliaPorIdQuery();
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(familia);
            var handler = new ObtenerFamiliaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerFamiliaPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarFamilia_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia= familia, IdFamilia= familia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia( command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(familia);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarFamilia_Error()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia = familia, IdFamilia = familia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(errorIsError);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarFamilia_NoExiste()
        {
            //Arrange

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresFamilia.NoEncontrada;

            //Act

            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia = familia, IdFamilia = familia.Id, ListaCambios= listaCambios };

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(errorIsError);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Familia no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarFamilia_DevuelveValidacionNombreError()
        {
            //Arrange
            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "".PadRight(51, 'a'),
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresFamilia.NombreInvalido;

            //Act
            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia = Familia, IdFamilia = Familia.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(Familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(error);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task EliminarFamilia_Ok()
        {
            //Arrange
            var id = 1;

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            //Act
            EliminarFamiliaCommand command = new EliminarFamiliaCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(id)).ReturnsAsync(familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarFamilia_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresFamilia.NoEncontrada;

            //Act
            EliminarFamiliaCommand command = new EliminarFamiliaCommand { Id = id};

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Familia no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarFamilia_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresFamilia.NoEncontrada;

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            //Act
            EliminarFamiliaCommand command = new EliminarFamiliaCommand { Id = id };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(id)).ReturnsAsync(familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Familia no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarFamilia_ErrorSustanciaControladaRelacionada()
        {
            //Arrange
            var id = 1;

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>()
            { 
                new Dominio.Entidades.SustanciaControlada()
                {
                    Id = 1,
                    ClasificacionArancelaria = "Clas",
                    ClasificacionAshrae = "Clas",
                    IdFamilia = 1,
                }                
            };

            //Act
            EliminarFamiliaCommand command = new EliminarFamiliaCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(id)).ReturnsAsync(familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.SustanciasControladasRelacionadas", result.Errors[0].Code);
            Assert.Equal("La(s) familias(s) tiene(n) sustancias controladas relacionadas", result.Errors[0].Description);
        }


        [Fact]
        public async Task CrearFamilia_Ok()
        {
            //Arrange
            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

            //Act
            
            CrearFamiliaCommand command = new CrearFamiliaCommand() { Familia = Familia };
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(Familia)).ReturnsAsync(Familia);

            var handler = new CrearFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Familia 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearFamilia_ErrorNombre()
        {
            //Arrange
            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = ""
            };

            //Act

            CrearFamiliaCommand command = new CrearFamiliaCommand() { Familia = Familia };
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(Familia)).ReturnsAsync(Familia);

            var handler = new CrearFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.NombreInvalido", result.FirstError.Code);
            Assert.Equal("La familia es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearFamilia_Error()
        {
            //Arrange
            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            var errorIsError = Error.Unexpected();

            //Act
            CrearFamiliaCommand command = new CrearFamiliaCommand() { Familia = Familia };
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(Familia)).ReturnsAsync(errorIsError);

            var handler = new CrearFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task EliminarFamilias_DevuelveOk()
        {
            // Arrange
            var idsFamilias = new List<int> { 1, 2, 3 };
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();
            
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(2)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(3)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarFamiliasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarFamiliasCommand { IdsFamilias = idsFamilias }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarFamilias_ErrorRelacionSustanciasControladas()
        {
            // Arrange
            var idsFamilias = new List<int> { 1, 2, 3 };
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada()
                {
                    Id = 1,
                    ClasificacionArancelaria = "Clas",
                    ClasificacionAshrae = "Clas",
                    IdFamilia = 1,
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarFamiliasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarFamiliasCommand { IdsFamilias = idsFamilias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.SustanciasControladasRelacionadas", result.Errors[0].Code);
            Assert.Equal("La(s) familias(s) tiene(n) sustancias controladas relacionadas", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarFamilias_DevuelveError()
        {
            // Arrange
            var idsFamilias = new List<int> { 1, 2, 3 };
            var erroror = ErroresFamilia.NoEncontrada;
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(3)).ReturnsAsync(erroror);

            var handler = new EliminarFamiliasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarFamiliasCommand { IdsFamilias = idsFamilias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarFamilias_CrearDevuelveError()
        {
            // Arrange

            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"

            };

            //Act           
            mockRepo.Setup(repo => repo.FamiliaRepository.ValidarFamilia(Familia.Id, Familia.Nombre)).ReturnsAsync(true);
            CrearFamiliaCommand command = new CrearFamiliaCommand() { Familia = Familia };
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(Familia)).ReturnsAsync(Familia);

            var handler = new CrearFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una familia con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarFamilias_EditarDevuelveExiste()
        {
            //Arrange

            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia = Familia, IdFamilia = Familia.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(Familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.ValidarFamilia(Familia.Id, Familia.Nombre)).ReturnsAsync(true);            
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(Familia);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una familia con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarFamilias_EditarDevuelveError()
        {
            //Arrange

            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };

           
            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarFamiliaCommand command = new EditarFamiliaCommand { Familia = Familia, IdFamilia = Familia.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamiliaPorId(1)).ReturnsAsync(Familia);
            mockRepo.Setup(repo => repo.FamiliaRepository.ValidarFamilia(Familia.Id, Familia.Nombre)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.FamiliaRepository.ActualizarFamilia(command.IdFamilia, command.ListaCambios, command.Familia)).ReturnsAsync(Familia);
            var handler = new EditarFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearFamilia_DevuelveValidacionNombreError()
        {
            //Arrange
            var Familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"

            };

            var error = ErroresFamilia.NombreInvalido;

            //Act

            CrearFamiliaCommand command = new CrearFamiliaCommand() { Familia = Familia };
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(Familia)).ReturnsAsync(error);

            var handler = new CrearFamiliaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        
        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() { new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia"
            }
            };
            var aduana = new Dominio.Entidades.Familia()
            {
                Id=1,
                Nombre = "Familia 1"
            };
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(It.IsAny<Dominio.Entidades.Familia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorSustanciasControladasRelacionadas()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() { 
                new Dominio.Entidades.Familia()
                {
                    Id = 1,
                    Nombre = "Familia"
                }
            };           
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada()
                {
                    Id = 1,
                    ClasificacionArancelaria = "Clas",
                    ClasificacionAshrae = "Clas",
                    IdFamilia = 1,
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(It.IsAny<Dominio.Entidades.Familia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.SustanciasControladasRelacionadas", result.Errors[0].Code);
            Assert.Equal("La(s) familias(s) tiene(n) sustancias controladas relacionadas", result.Errors[0].Description);
        }


        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Familia>() {
                new Dominio.Entidades.Familia()
                {
                    Id = 1,
                    Nombre = "Familia"
                }
            };

            var error = ErroresFamilia.DatosDuplicados;
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.ValidarFamilia(datos[0].Id, datos[0].Nombre)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Familia.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe una familia con los datos proporcionados", result.FirstError.Description);
        }
        [Fact]
        public async Task ImportarDatos_ObtenerFamiliasDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() 
            { 
                new Dominio.Entidades.Familia()
                {
                    Id = 1,
                    Nombre= "Familia"
                }
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(ErrorOr.Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarFamiliaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() { new Dominio.Entidades.Familia()
            {
                Id = -1,
                Nombre= "Familia"
                }
            };
            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(-1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearFamiliaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() 
            { 
                new Dominio.Entidades.Familia()
                {
                    Id = -1,
                    Nombre= "Familia"
                }
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(It.IsAny<Dominio.Entidades.Familia>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatosExcedenLimiteNombre_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>() 
            { 
                new Dominio.Entidades.Familia()
                {
                    Id = 1,
                    Nombre= "Nombre de Familia Excede Nombre de Familia Excede Nombre de Familia Excede Nombre de Familia Excede Nombre de Familia Excede"
                }
            };

            var sustanciaC = new List<Dominio.Entidades.SustanciaControlada>();
            var error = ErroresFamilia.NombreInvalido;

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaC);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia() { Nombre = "A" },
                new Dominio.Entidades.Familia() { Nombre = "A" }
            };

            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.FamiliaRepository.EliminarFamilia(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.CrearFamilia(It.IsAny<Dominio.Entidades.Familia>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
