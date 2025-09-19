using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladas;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Queries.ObtenerSustanciaControladaPorId;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EditarSustanciaControlada;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciaControlada;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.EliminarSustanciasControladas;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.CrearSustanciaControlada;
using ErrorOr;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class SustanciaControladaTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public SustanciaControladaTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerSustanciaControladas_Ok()
        {
            //Act
            var SustanciaControladas = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada()
                {
                    Id =1,
                    ClasificacionArancelaria = "Clasificacion 1",
                    ClasificacionAshrae =  "D650",
                    IdFamilia = 1,
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 4"
                }
            };

            ObtenerSustanciaControladasQuery obtenerCasaQuery = new ObtenerSustanciaControladasQuery();
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(SustanciaControladas);
            var handler = new ObtenerSustanciaControladasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.SustanciaControlada>>(result.Value);
        }

        [Fact]
        public async Task ObtenerSustanciaControladaPorId_Ok()
        {
            //Arange
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4"
            };

            //Act
            ObtenerSustanciaControladaPorIdQuery obtenerSustanciaControladasPorIdQuery = new ObtenerSustanciaControladaPorIdQuery();
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            var handler = new ObtenerSustanciaControladaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerSustanciaControladasPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarSustanciaControlada_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                TipoGas = "GAS 1"
            };

            var listaCambios = new List<string> { "Codigo"};

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada= SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada( command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(SustanciaControlada);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarSustanciaControlada_ValidacionDuplicado_DevuelveError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae = "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                TipoGas = "GAS 1"
            };

            var listaCambios = new List<string> { "Codigo" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada = SustanciaControlada, IdSustanciaControlada = SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository
                .ValidarSustanciasControladas(It.IsAny<int>(), It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(true); 
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarSustanciaControladaClasificacionArancelaria_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1",
                ClasificacionAshrae =  "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4"
            };

            var listaCambios = new List<string> { "ClasificacionArancelaria" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada= SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(SustanciaControlada);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarSustanciaControladaClasificacionAshrae_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650123456D650123456D650123456D650123456D650123456D650123456D650123456D650123456D650123456D6501234561",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4"
            };

            var listaCambios = new List<string> { "ClasificacionAshrae" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada= SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(SustanciaControlada);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarSustanciaPotencialCalentamientoGlobal_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4"
            };

            var listaCambios = new List<string> { "PotencialCalentamientoGlobal" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada= SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(SustanciaControlada);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarSustanciaTipoGasMax()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae = "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                TipoGas = new string('G', 101)  
            };

            var listaCambios = new List<string> { "TipoGas" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada= SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(SustanciaControlada);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarCasa_Error()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                IdFamilia = 1,
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                TipoGas = "Gas 1"

            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada = SustanciaControlada, IdSustanciaControlada = 1, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(errorIsError);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarSustanciaControlada_NoExiste()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",              
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAS 1"

            };
            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresSustanciaControlada.NoEncontrada;

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada= SustanciaControlada, IdSustanciaControlada =SustanciaControlada.Id, ListaCambios= listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(errorIsError);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Sustancia controlada no encontrada", result.FirstError.Description);
        }


        [Fact]
        public async Task EliminarSustanciaControlada_Ok()
        {
            //Arrange
            var id = 1;

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia

            };

            //Act
            EliminarSustanciaControladaCommand command = new EliminarSustanciaControladaCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(id)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarSustanciaControlada_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresSustanciaControlada.NoEncontrada;

            //Act
            EliminarSustanciaControladaCommand command = new EliminarSustanciaControladaCommand { Id = id};

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Sustancia controlada no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarSustanciaControlada_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresSustanciaControlada.NoEncontrada;

            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia

            };

            //Act
            EliminarSustanciaControladaCommand command = new EliminarSustanciaControladaCommand { Id = id };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(id)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Sustancia controlada no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearSustanciaControlada_Ok()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAS 1"

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Clasificacion 1", result.Value.ClasificacionArancelaria);
        }
        [Fact]
        public async Task CrearSustanciaControlada_ClasificacionArancelariaError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
          
        }
        [Fact]
        public async Task CrearSustanciaControlada_ClasificacionAshraeError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650123456D650123456D650123456D650123456D650123456D650123456D650123456D650123456D650123456D6501234561",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearSustanciaControlada_PotencialCalentamientoGlobalError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4Calentamiento Nivel 4",
                Familia = familia

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearSustanciaControladaObtenerFamiliasError()
        {
            //Arrange
            var error = ErroresSustanciaControlada.FamiliaNoEncontrada;
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAS 1"

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerFamiliaSustanciaControlada(It.IsAny<int>())).ReturnsAsync(error);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            
        }
        [Fact]
        public async Task CrearSustanciaControlada_TipoGasError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1n 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = new string('G', 101)

            };

            //Act            
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(SustanciaControlada);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearSustanciaControlada_Error()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "Tipo GAS 1"

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(errorIsError);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }           

        [Fact]
        public async Task Obtenerfamilias_EditarDevuelveError()
        {
            //Arrange
            var familiaError = ErroresSustanciaControlada.FamiliaNoEncontrada;
            var unexpectedError = ErrorOr.Error.Unexpected();
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                IdFamilia = 2
                

            };

            var listaCambios = new List<string> { "IdFamilia" };

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada = SustanciaControlada, IdSustanciaControlada = SustanciaControlada.Id, ListaCambios = listaCambios };
            
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerFamiliaSustanciaControlada(1)).ReturnsAsync(unexpectedError);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(unexpectedError);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

             
        [Fact]
        public async Task CrearSustanciaControlada_DevuelveValidacionError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAs 1"

            };

            var error =ErroresSustanciaControlada.DatosDuplicados;

            //Act

            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(SustanciaControlada)).ReturnsAsync(error);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearSustanciaControlada_ValidarDuplicado_DevuelveError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae = "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas = "GAs 1"

            };

            var error = ErroresSustanciaControlada.DatosDuplicados;

            //Act

            CrearSustanciaControladaCommand command = new CrearSustanciaControladaCommand() { SustanciaControlada = SustanciaControlada };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository
            .ValidarSustanciasControladas(It.IsAny<int>(), It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(true);

            var handler = new CrearSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }


        [Fact]
        public async Task ActualizarSustanciaControlada_DevuelveValidacionNombreError()
        {
            //Arrange
            var familia = new Dominio.Entidades.Familia()
            {
                Id = 1,
                Nombre = "Familia 1"
            };
            var SustanciaControlada = new Dominio.Entidades.SustanciaControlada()
            {
                Id = 1,
                ClasificacionArancelaria = "Clasificacion 1",
                ClasificacionAshrae =  "D650",
                PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                Familia = familia,
                TipoGas= "GAS1"

            };

            var listaCambios = new List<string> { "Codigo" };

            var error = ErroresSustanciaControlada.DatosDuplicados;

            //Act
            EditarSustanciaControladaCommand command = new EditarSustanciaControladaCommand { SustanciaControlada = SustanciaControlada, IdSustanciaControlada = SustanciaControlada.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(1)).ReturnsAsync(SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ActualizarSustanciaControlada(command.IdSustanciaControlada, command.ListaCambios, command.SustanciaControlada)).ReturnsAsync(error);
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        [Fact]
        public async Task EditarSustanciaControlada_FamiliaError()
        {
            // Arrange
            var errorIsError = ErroresSustanciaControlada.NoEncontrada;
            var command = new EditarSustanciaControladaCommand
            {
                IdSustanciaControlada = 1,
                ListaCambios = new List<string> { "ClasificacionArancelaria" },
                SustanciaControlada = new Dominio.Entidades.SustanciaControlada
                {
                    Id = 1,
                    ClasificacionArancelaria = "Clasificacion 1",
                    ClasificacionAshrae = "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                    TipoGas = "Gas 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(command.IdSustanciaControlada))
                    .ReturnsAsync(errorIsError);

            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("SustanciaControlada.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Sustancia controlada no encontrada", result.FirstError.Description);
        }
        [Fact]
        public async Task EditarSustanciaControlada_ObtenerFamiliaError()
        {
            // Arrange
            var errorIsError = ErroresSustanciaControlada.NoEncontrada;
            var command = new EditarSustanciaControladaCommand
            {
                IdSustanciaControlada = 1,
                ListaCambios = new List<string> { "IdFamilia" },
                SustanciaControlada = new Dominio.Entidades.SustanciaControlada
                {
                    Id = 1,
                    ClasificacionArancelaria = "Clasificacion 1",
                    ClasificacionAshrae = "D650",
                    PotencialCalentamientoGlobal = "Calentamiento Nivel 4",
                    TipoGas = "Gas 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladaPorId(command.IdSustanciaControlada))
                    .ReturnsAsync(command.SustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerFamiliaSustanciaControlada(It.IsAny<int>())).ReturnsAsync(errorIsError);
           
            
            var handler = new EditarSustanciaControladaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminarSustanciaControlada_DevuelveOk()
        {
            // Arrange
            var idsSustanciasControladas = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarSustanciasControladasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarSustanciasControladasCommand { IdsSustanciasControladas = idsSustanciasControladas }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarSustanciaControlada_DevuelveError()
        {
            // Arrange
            var idsSustanciasControladas = new List<int> { 1, 2, 3 };
            var erroror = ErroresSustanciaControlada.NoEncontrada;

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(3)).ReturnsAsync(erroror);

            var handler = new EliminarSustanciasControladasCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarSustanciasControladasCommand { IdsSustanciasControladas = idsSustanciasControladas }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria",
                ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                TipoGas = "Gas 1"
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = "Gas 1"
                    
                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(sustanciaControlada[0])).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarDuplicados_DevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria",
                ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                TipoGas = "Gas 1"
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = "Gas 1"

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository
                .ValidarSustanciasControladas(It.IsAny<int>(), It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_SustanciaControladaClasificacionArancelariaTamanoDevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelariaClasificacionArancelariaClasificacionArancelariaClasificacionArancelariaClasificacionArancelaria",
                ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                TipoGas = "Gas 1"
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = "Gas 1"

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_SustanciaControladaClasificacionAshraeDevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria 1",
                ClasificacionAshrae="Clasificaion AshraeClasificaion AshraeClasificaion AshraeClasificaion AshraeClasificaion AshraeClasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal 1",
                TipoGas = "Gas 1"
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = "Gas 1"

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_SustanciaControladaPotencialCalentamientoGlobalTamanoDevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria 1",
                ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobalPotencialCalentamientoGlobalPotencialCalentamientoGlobalPotencialCalentamientoGlobal",
                TipoGas = "Gas 1"
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = "Gas 1"

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_SustanciaControladaTipoGasTamanoDevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                Familia="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria 1",
                ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGloball",
                TipoGas = new string('G', 101)
            }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria",
                    ClasificacionAshrae="Clasificaion Ashrae",
                    PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                    IdFamilia=1,
                    TipoGas = new string('G', 101)

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveDuplicadoOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() {
                new ImportarSustanciasControladasCommandDto()
                {
                    Familia ="Familia 1",
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                },
                 new ImportarSustanciasControladasCommandDto()
                {
                    Familia ="Familia 1",
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };

            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            var error = ErroresSustanciaControlada.DatosDuplicados;
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
          
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("SustanciaControlada.DatosDuplicadosArchivo", result.FirstError.Code);
            Assert.Equal("Ya existe una sustancia controlada con los datos proporcionados", result.FirstError.Description);
        }
        [Fact]
        public async Task ImportarDatos_ObtenerFamiliasDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                 Familia ="Familia 1",
               ClasificacionArancelaria ="ClasificacionArancelaria"
            }
            };

            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(ErrorOr.Error.Failure());

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
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                 Familia ="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(It.IsAny<int>())).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

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
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                 Familia ="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria"
            }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 2"
                }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

      
        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarSustanciasControladasCommandDto>()
            {
                new ImportarSustanciasControladasCommandDto() { ClasificacionArancelaria = "ClasificacionArancelaria" },
                new ImportarSustanciasControladasCommandDto() {ClasificacionArancelaria = "ClasificacionArancelaria" }
            };

            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.EliminarSustanciaControlada(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(sustanciaControlada[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarSustanciasControladasCommandDto>() { new ImportarSustanciasControladasCommandDto()
            {
                 Familia ="Familia 1",
                ClasificacionArancelaria ="ClasificacionArancelaria",
                  ClasificacionAshrae="Clasificaion Ashrae",
                PotencialCalentamientoGlobal= "PotencialCalentamientoGlobal",
                TipoGas = "TipoGas",

                }
            };
            var familias = new List<Dominio.Entidades.Familia>()
            {
                new Dominio.Entidades.Familia
                {
                    Id =1,
                    Nombre= "Familia 1"
                }
            };
            var sustanciaControlada = new List<Dominio.Entidades.SustanciaControlada>()
            {
                new Dominio.Entidades.SustanciaControlada{
                    Id=1,
                    ClasificacionArancelaria ="ClasificacionArancelaria"
                }
            };
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.ObtenerSustanciaControladas()).ReturnsAsync(sustanciaControlada);
          
            mockRepo.Setup(repo => repo.FamiliaRepository.ObtenerFamilias()).ReturnsAsync(familias);
            mockRepo.Setup(repo => repo.SustanciaControladaRepository.CrearSustanciaControlada(It.IsAny<Dominio.Entidades.SustanciaControlada>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

    }
}
