using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductos;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Queries.ObtenerTipoProductoPorId;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EditarTipoProducto;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.EliminarTipoProducto;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.TipoProducto.Commands.CrearTipoProducto;
using ErrorOr;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.EliminarTipoProductos;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.TipoProductos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class TipoProductoTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        private Mock<IAccesoGestionUsuariosService> mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();


        public TipoProductoTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        }
        [Fact]
        public async Task ObtenerTipoProductos_Ok()
        {
            //Act
            var TipoProductos = new List<Dominio.Entidades.TipoProducto>()
            {
                new Dominio.Entidades.TipoProducto()
                {
                    Id =1,
                    IdCategoria=1,
                    Tipo = "Tipo 1",
                    IdInstitucion = 1
                }
            };

            ObtenerTipoProductosQuery obtenerCasaQuery = new ObtenerTipoProductosQuery();
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            var handler = new ObtenerTipoProductosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCasaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.TipoProducto>>(result.Value);
        }

        [Fact]
        public async Task ObtenerTipoProductoPorId_Ok()
        {
            //Arange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            //Act
            ObtenerTipoProductoPorIdQuery obtenerTipoProductosPorIdQuery = new ObtenerTipoProductoPorIdQuery();
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            var handler = new ObtenerTipoProductoPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerTipoProductosPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarTipoProducto_Ok()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProductoNuevo = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 2,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };

            var listaCambios = new List<string> { "Tipo", "IdInstitucion", "IdCategoria" };

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProductoNuevo, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.VerificaExistenciaPorTipoProducto(1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(TipoProducto);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ActualizarTipoProductoIdInstitucion_Error()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {                
                IdInstitucion = 2
            };

            var listaCambios = new List<string> { "IdInstitucion" };

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto1, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(TipoProducto);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarTipoProductoIdInstitucion_ExisteCaracteristica()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5,
                CaracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>()
                {
                    new Dominio.Entidades.CaracteristicaTipoProducto()
                    {
                        Id = 1,
                        IdCaracteristica = 1,
                        IdTipoProducto = 1
                    }
                }
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {                
                IdInstitucion = 7,
                CaracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>()
                {
                    new Dominio.Entidades.CaracteristicaTipoProducto()
                    {
                        Id = 1,
                        IdCaracteristica = 1,
                        IdTipoProducto = 1
                    }
                }
            };


            var listaCambios = new List<string> { "IdInstitucion" };

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto2, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.VerificaExistenciaPorTipoProducto(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(TipoProducto);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarTipoProducto_Error()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto, IdTipoProducto = 1, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(errorIsError);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarTipoProducto_NoExiste()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };
            var listaCambios = new List<string> { "Codigo" };
            var errorIsError = ErroresTipoProducto.NoEncontrado;

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(errorIsError);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Tipo producto no encontrado", result.FirstError.Description);
        }


        [Fact]
        public async Task EliminarTipoProducto_Ok()
        {
            //Arrange
            var id = 1;

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            //Act
            EliminarTipoProductoCommand command = new EliminarTipoProductoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(id)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarTipoProductoProductoRequisitoExiste()
        {
            //Arrange
            var id = 1;

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            //Act
            EliminarTipoProductoCommand command = new EliminarTipoProductoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(id)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarExisteProductoRequisito(id)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminarTipoProducto_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresTipoProducto.NoEncontrado;

            //Act
            EliminarTipoProductoCommand command = new EliminarTipoProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Tipo producto no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarTipoProducto_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresTipoProducto.NoEncontrado;

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            //Act
            EliminarTipoProductoCommand command = new EliminarTipoProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(id)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Tipo producto no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearTipoProducto_Ok()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5

            };

            //Act            
            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(TipoProducto);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Tipo 1", result.Value.Tipo);
        }

        [Fact]
        public async Task CrearTipoProducto_Error()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(errorIsError);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task CrearTipoProductoIdInstitucion_Error()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            //Act            
            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(TipoProducto);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearTipoProductoLongitugMaxima_Error()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = new string('a', 201),
                IdInstitucion = 5

            };

            //Act            
            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(TipoProducto);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ValidarTipoProducto_CrearDevuelveError()
        {
            // Arrange

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1

            };

            //Act           
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarTipoProducto(TipoProducto.Tipo, TipoProducto.IdCategoria, TipoProducto.IdInstitucion, TipoProducto.Id)).ReturnsAsync(true);
            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(TipoProducto);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un tipo producto con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarTipo_EditarDevuelveExiste()
        {
            //Arrange

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };


            var listaCambios = new List<string> { "Tipo" };

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarTipoProducto(TipoProducto.Tipo, TipoProducto.IdCategoria, TipoProducto.IdInstitucion, TipoProducto.Id)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(TipoProducto);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un tipo producto con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarTipo_EditarDevuelveError()
        {
            //Arrange

            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };


            var listaCambios = new List<string> { "Tipo" };

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarTipoProducto(TipoProducto.Tipo, TipoProducto.IdCategoria, TipoProducto.IdInstitucion, TipoProducto.Id)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(TipoProducto);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task CrearTipoProducto_DevuelveValidacionNombreError()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                Tipo = new string('a', 201),
                IdCategoria = 1,
                IdInstitucion = 1

            };

            var error = ErroresTipoProducto.TipoTamano;

            //Act

            CrearTipoProductoCommand command = new CrearTipoProductoCommand() { TipoProducto = TipoProducto };
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(TipoProducto)).ReturnsAsync(error);

            var handler = new CrearTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }




        [Fact]
        public async Task ActualizarTipoProducto_DevuelveValidacionNombreError()
        {
            //Arrange
            var TipoProducto = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                Tipo = new string('*', 201),
                IdCategoria = 1,
                IdInstitucion = 1
            };

            var listaCambios = new List<string> { "Tipo" };

            var error = ErroresTipoProducto.TipoTamano;

            //Act
            EditarTipoProductoCommand command = new EditarTipoProductoCommand { TipoProducto = TipoProducto, IdTipoProducto = TipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ActualizarTipoProducto(command.IdTipoProducto, command.ListaCambios, command.TipoProducto)).ReturnsAsync(error);
            var handler = new EditarTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task EliminadoMasivoTipoProducto_DevuelveOk()
        {
            // Arrange
            var idsTipoProductos = new List<int> { 1, 2 };



            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 1
            };

            var lstTipoProductos = new List<Dominio.Entidades.TipoProducto>();
            lstTipoProductos.Add(TipoProducto1);
            lstTipoProductos.Add(TipoProducto2);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(lstTipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto1);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(2)).ReturnsAsync(TipoProducto2);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarTipoProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarTipoProductosCommand { IdsTipoProductos = idsTipoProductos }, default);

            // Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task EliminadoMasivoTipoProductoProductoExiste()
        {
            // Arrange
            var idsTipoProductos = new List<int> { 1, 2 };



            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 1
            };

            var lstTipoProductos = new List<Dominio.Entidades.TipoProducto>();
            lstTipoProductos.Add(TipoProducto1);
            lstTipoProductos.Add(TipoProducto2);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(lstTipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto1);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(2)).ReturnsAsync(TipoProducto2);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarExisteProductoRequisito(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(2)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new EliminarTipoProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarTipoProductosCommand { IdsTipoProductos = idsTipoProductos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoTipoProducto_ErrorEliminar()
        {
            // Arrange
            var idsTipoProductos = new List<int> { 1, 2 };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };
            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var lstTipoProductos = new List<Dominio.Entidades.TipoProducto>();
            lstTipoProductos.Add(TipoProducto1);
            lstTipoProductos.Add(TipoProducto2);

            var erroror = ErroresTipoProducto.NoEncontrado;

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(lstTipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(TipoProducto1);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(2)).ReturnsAsync(TipoProducto2);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(2)).ReturnsAsync(erroror);

            var handler = new EliminarTipoProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarTipoProductosCommand { IdsTipoProductos = idsTipoProductos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Tipo producto no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>()
            {
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 1",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                    IdInstitucion = 5
                }
            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };

            // Aquí el cambio clave: asignamos IdInstitucion para que coincida con la institución de datos
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",   // Sin espacios extras
                IdInstitucion = 5         // <- Muy importante para que pase la validación
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto> { TipoProducto1, TipoProducto2 };
            var categorias = new List<Dominio.Entidades.Categoria> { categoria };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);

            var institucion = new List<InstitucionDto>()
            {
                new InstitucionDto()
                {
                    Id = 5,
                    Nombre = "DCA"
                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorTipoProductoInstitucion()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 1",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",

                }
            };
            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 ",
                IdInstitucion = 7
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);
            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task ImportarDatos_DevuelveExistenDuplicadosPorCodigo()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() {
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                },
                new ImportarTipoProductoCommandDto()
                {
                   Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                }
            };

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorInstitucionInexistente()
        {
            // Arrange
            var modo = 2;

            var datos = new List<ImportarTipoProductoCommandDto>() {
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                },
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                }
            };


            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);


            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_ErrorInstitucionFalloServicio()
        {
            // Arrange
            var modo = 2;

            var datos = new List<ImportarTipoProductoCommandDto>() {
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                },
                new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 02",
                    Tipo = "Tipo 2",
                    Institucion = "DCA",
                }
            };
            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);


            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);


            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

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

            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "Institucion 1",
                }
            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarTipoProducto("01", 1, 1, 0)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);

            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

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
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                Categoria = "Categoria 01",
                Tipo = "Tipo 1",
                Institucion = "Institucion 1",
                }
            };



            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };

            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };


            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);


            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);

            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);


            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "MINCEX"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorCategoriaPerteneceAOtraInstitucion()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>()
                {
                    new ImportarTipoProductoCommandDto()
                    {
                        Categoria = "Categoria 01",
                        Tipo = "Tipo 1",
                        Institucion = "Institucion 1",
                    }
                };

            var instituciones = new List<InstitucionDto>
            {
                new InstitucionDto { Id = ConstantesInstituciones.DCA, Nombre = "Institucion 1" }
            };

            var categorias = new List<Dominio.Entidades.Categoria>
                {
                    new Dominio.Entidades.Categoria { Id = 1, Nombre = "Categoria 01", IdInstitucion = 999 } 
                };

            mockAccesoGestionUsuariosService
                .Setup(s => s.ObtenerInstituciones())
                .ReturnsAsync(instituciones);

            mockRepo.Setup(r => r.CategoriaRepository.ObtenerCategorias())
                .ReturnsAsync(categorias);

            mockRepo.Setup(r =>
                r.TipoProductoRepository.ValidarTipoProducto(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>()
                    )).ReturnsAsync(false);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(result.Errors, e => e.Code == "TipoProducto.CategoriaPerteneceAOtraInstitucion");
        }

        [Fact]
        public async Task ImportarDatos_ErrorObtenerTipoProductos()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                }

            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };


            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var Categorias = new List<Dominio.Entidades.Categoria>(){
                new Dominio.Entidades.Categoria()
                {
                    Id = 1,
                    Nombre = "Categoria 01",
                    IdInstitucion = 5
                }
            };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(Categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorEliminarTipoProducto()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    IdInstitucion = 5
                }
            };


            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var Categorias = new List<Dominio.Entidades.Categoria>(){
                new Dominio.Entidades.Categoria()
                {
                    Id = 1,
                    Nombre = "Categoria 01",
                    IdInstitucion = 5
                }
            };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(Categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.Errors[0].Code);
            Assert.Equal("A failure has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ImportarDatos_ErrorExisteRequisito()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                }

            };


            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var Categorias = new List<Dominio.Entidades.Categoria>(){
                new Dominio.Entidades.Categoria()
                {
                    Id = 1,
                    Nombre = "Categoria 01",
                    IdInstitucion = 5
                }
            };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarExisteProductoRequisito(1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(Categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert            
            Assert.True(result.IsError);
            Assert.Equal("TipoProducto.ProductoRequisitoExiste", result.Errors[0].Code);
            Assert.Equal("Producto requisito con tipo producto asociado", result.Errors[0].Description);
        }


        [Fact]
        public async Task ImportarDatos_ErrorCrear()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                }
            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };


            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);


            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);

            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(Error.Failure());

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);


            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearConEliminar_OK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 01",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",
                }
            };

            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 1
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 1",
                IdInstitucion = 1
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };


            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);


            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);

            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository
            .CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(It.IsAny<Dominio.Entidades.TipoProducto>());

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);


            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_TipoProductoLongitudMaximaError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 1",
                    Tipo = new string('a',201),
                    Institucion = "DCA",

                }
            };
            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_TipoProductoInstitucionError()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 1",
                    Tipo = "Tipo 1",
                    Institucion = "DCA TEST",

                }
            };
            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 1,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);
            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);

            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.CrearTipoProducto(It.IsAny<Dominio.Entidades.TipoProducto>())).ReturnsAsync(TipoProductos[0]);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "DCA TEST"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveErrorDatoDuplicado()
        {
            // Arrange
            var modo = 1;
            var datos = new List<ImportarTipoProductoCommandDto>() { new ImportarTipoProductoCommandDto()
                {
                    Categoria = "Categoria 1",
                    Tipo = "Tipo 1",
                    Institucion = "DCA",

                }
            };
            var TipoProducto1 = new Dominio.Entidades.TipoProducto()
            {
                Id = 1,
                IdCategoria = 1,
                Tipo = "Tipo 1",
                IdInstitucion = 5
            };

            var TipoProducto2 = new Dominio.Entidades.TipoProducto()
            {
                Id = 2,
                IdCategoria = 2,
                Tipo = "Tipo 2",
                IdInstitucion = 5
            };
            var categoria = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 1 "
            };

            var TipoProductos = new List<Dominio.Entidades.TipoProducto>();
            TipoProductos.Add(TipoProducto1);
            TipoProductos.Add(TipoProducto2);
            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria);

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(TipoProductos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.EliminarTipoProducto(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ValidarTipoProducto(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(true);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
    }
}
