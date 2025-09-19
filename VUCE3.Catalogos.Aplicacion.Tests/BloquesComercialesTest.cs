using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Queries.ObtenerBloqueComercialPorId;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EditarBloqueComercial;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EliminarBloqueComercial;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.CrearBloqueComercial;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.EliminarBloquesComerciales;
using VUCE3.Catalogos.Aplicacion.BloquesComerciales.Commands.Importardatos;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class BloqueComercialTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public BloqueComercialTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerBloquesComerciales_Ok()
        {
            //Act
            var BloquesComerciales = new List<Dominio.Entidades.BloqueComercial>()
            {
                new Dominio.Entidades.BloqueComercial()
                {
                    Id =1,
                    Nombre = "Bloque USA",
                }
            };

            ObtenerBloquesComercialesQuery obtenerCasaQuery = new ObtenerBloquesComercialesQuery();
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(BloquesComerciales);
            var handler = new ObtenerBloquesComercialesQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.BloqueComercial>>(result.Value);
        }

        [Fact]
        public async Task ObtenerBloqueComercialPorId_Ok()
        {
            //Arange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,                
                Nombre = "Bloque USA",
             
            };

            //Act
            ObtenerBloqueComercialPorIdQuery obtenerBloquesComercialesPorIdQuery = new ObtenerBloqueComercialPorIdQuery();
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            var handler = new ObtenerBloqueComercialPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerBloquesComercialesPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarBloqueComercial_Ok()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,            
                Nombre = "Bloque USA",
            
            };

            var listaCambios = new List<string> { "Nombre"};

            //Act
            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial= BloqueComercial, IdBloqueComercial= BloqueComercial.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial( command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(BloqueComercial);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCasa_Error()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,            
                Nombre = "BloqueComercial 1",
                

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial = BloqueComercial, IdBloqueComercial = 1, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(errorIsError);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarBloqueComercial_NoExiste()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,              
                Nombre = "BloqueComercial 1",
               

            };
            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresBloqueComercial.NoEncontrada;

            //Act
            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial= BloqueComercial, IdBloqueComercial =BloqueComercial.Id, ListaCambios= listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(errorIsError);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("BloqueComercial no encontrada", result.FirstError.Description);
        }


        [Fact]
        public async Task EliminarBloqueComercial_Ok()
        {
            //Arrange
            var id = 1;

            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
              

            };

            //Act
            EliminarBloqueComercialCommand command = new EliminarBloqueComercialCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(id)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarBloqueComercial_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresBloqueComercial.NoEncontrada;

            //Act
            EliminarBloqueComercialCommand command = new EliminarBloqueComercialCommand { Id = id};

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("BloqueComercial no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarBloqueComercial_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresBloqueComercial.NoEncontrada;

            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
               
            };

            //Act
             EliminarBloqueComercialCommand command = new EliminarBloqueComercialCommand { Id = id };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(id)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", result.FirstError.Code);
            Assert.Equal("BloqueComercial no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearBloqueComercial_Ok()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,                
                Nombre = "BloqueComercial 1",
                

            };

            //Act            
            CrearBloqueComercialCommand command = new CrearBloqueComercialCommand() { BloqueComercial = BloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(BloqueComercial)).ReturnsAsync(BloqueComercial);

            var handler = new CrearBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("BloqueComercial 1", result.Value.Nombre);
        }           

        [Fact]
        public async Task CrearBloqueComercial_Error()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,                
                Nombre = "BloqueComercial 1",
     
            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearBloqueComercialCommand command = new CrearBloqueComercialCommand() { BloqueComercial = BloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(BloqueComercial)).ReturnsAsync(errorIsError);

            var handler = new CrearBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }           

        [Fact]
        public async Task ValidarBloqueComercial_CrearDevuelveError()
        {
            // Arrange

            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
             

            };

            //Act           
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ValidarBloqueComercial(BloqueComercial.Id, BloqueComercial.Nombre)).ReturnsAsync(true);
            CrearBloqueComercialCommand command = new CrearBloqueComercialCommand() { BloqueComercial = BloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(BloqueComercial)).ReturnsAsync(BloqueComercial);

            var handler = new CrearBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un bloque comercial con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCasas_EditarDevuelveExiste()
        {
            //Arrange

            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
              
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarBloqueComercialCommand  command = new EditarBloqueComercialCommand { BloqueComercial = BloqueComercial, IdBloqueComercial = BloqueComercial.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ValidarBloqueComercial(BloqueComercial.Id, BloqueComercial.Nombre)).ReturnsAsync(true);            
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(BloqueComercial);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un bloque comercial con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCasas_EditarDevuelveError()
        {
            //Arrange

            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
               
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial = BloqueComercial, IdBloqueComercial = BloqueComercial.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ValidarBloqueComercial(BloqueComercial.Id, BloqueComercial.Nombre)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(BloqueComercial);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }
                     
        [Fact]
        public async Task CrearBloqueComercial_DevuelveErrorDatosDuplicados()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
            };

            var error =ErroresBloqueComercial.DatosDuplicados;

            //Act

            CrearBloqueComercialCommand command = new CrearBloqueComercialCommand() { BloqueComercial = BloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(BloqueComercial)).ReturnsAsync(error);

            var handler = new CrearBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearBloqueComercial_DevuelveErrorNombreInvalido()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "".PadRight(301, 'a'),
            };

            //Act

            CrearBloqueComercialCommand command = new CrearBloqueComercialCommand() { BloqueComercial = BloqueComercial };
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(BloqueComercial)).ReturnsAsync(BloqueComercial);

            var handler = new CrearBloqueComercialCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(ErroresBloqueComercial.NombreInvalido, result.FirstError);
        }

        [Fact]
        public async Task ActualizarBloqueComercial_DevuelveErrorDatosDuplicados()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "BloqueComercial 1",
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresBloqueComercial.DatosDuplicados;

            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial = BloqueComercial, IdBloqueComercial = BloqueComercial.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(error);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarBloqueComercial_DevuelveErrorNombreInvalido()
        {
            //Arrange
            var BloqueComercial = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "".PadRight(301, 'a'),
            };

            var listaCambios = new List<string> { "Nombre" };

            EditarBloqueComercialCommand command = new EditarBloqueComercialCommand { BloqueComercial = BloqueComercial, IdBloqueComercial = BloqueComercial.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(BloqueComercial);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial)).ReturnsAsync(BloqueComercial);
            var handler = new EditarBloqueComercialCommandHandler(mockRepo.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresBloqueComercial.NombreInvalido, result.FirstError);
        }

        [Fact]
        public async Task EliminadoMasivoBloqueComercial_DevuelveOk()
        {
            // Arrange
            var idsBloquesComerciales = new List<int> { 1, 2 };

            var bloque1 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Nombre 1"
            };

            var bloque2 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 2,
                Nombre = "Nombre 2"
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(bloque1);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(2)).ReturnsAsync(bloque2);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarBloquesComercialesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBloquesComercialesCommand { IdsBloquesComerciales = idsBloquesComerciales }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoBloquesComerciales_ErrorObtenerBloqueComercialPorId()
        {
            // Arrange
            var idsBloquesComerciales = new List<int> { 1, 2 };

            var bloque1 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Nombre 1"
            };

            var bloque2 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 2,
                Nombre = "Nombre 2"
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(2)).ReturnsAsync(bloque2);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(2)).ReturnsAsync(It.IsAny<Deleted>());


            var handler = new EliminarBloquesComercialesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBloquesComercialesCommand { IdsBloquesComerciales = idsBloquesComerciales }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoBloquesComerciales_ErrorEliminar()
        {
            // Arrange
            var idsBloquesComerciales = new List<int> { 1, 2 };

            var bloque1 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 1,
                Nombre = "Nombre 1"
            };

            var bloque2 = new Dominio.Entidades.BloqueComercial()
            {
                Id = 2,
                Nombre = "Nombre 2"
            };

            var erroror = ErroresBloqueComercial.NoEncontrada;

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(1)).ReturnsAsync(bloque1);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloqueComercialPorId(2)).ReturnsAsync(bloque2);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(2)).ReturnsAsync(erroror);

            var handler = new EliminarBloquesComercialesCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarBloquesComercialesCommand { IdsBloquesComerciales = idsBloquesComerciales }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("BloqueComercial.NoEncontrada", result.Errors[0].Code);
            Assert.Equal("BloqueComercial no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre 1"       
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveExistenDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.BloqueComercial>() {
                new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre 1"
                },
                new Dominio.Entidades.BloqueComercial()
                {
                    Id = 2,
                    Nombre = "Nombre 1"
                }

            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

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
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ValidarBloqueComercial(1, "Nombre 1")).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

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
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre =  "".PadRight(301, 'a')
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerBloquesComerciales()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre 1"                   
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarBloquesComerciales()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre"                    
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(datos[0]);

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
            var datos = new List<Dominio.Entidades.BloqueComercial>() { new Dominio.Entidades.BloqueComercial()
                {
                    Id = 1,
                    Nombre = "Nombre 1"
                }
            };

            mockRepo.Setup(repo => repo.BloqueComercialRepository.ObtenerBloquesComerciales()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.EliminarBloqueComercial(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.BloqueComercialRepository.CrearBloqueComercial(It.IsAny<Dominio.Entidades.BloqueComercial>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
