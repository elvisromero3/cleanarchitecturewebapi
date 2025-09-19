using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitosPorId;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Queries.ObtenerProductoRequisitos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EditarProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisito;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.EliminarProductoRequisitos;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Commands.CrearProductoRequisito;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Aplicacion.ProductoRequisito.Command.EliminarProdcutoRequisitos;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ProductoRequisitoTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public ProductoRequisitoTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerProductoRequisitos_Ok()
        {
            //Act
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1 , IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito = new List<Dominio.Entidades.ProductoRequisito>()
            {
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                   IdRequisito=1,
                   IdTipoProducto=1
                }
            };

            ObtenerProductoRequisitoQuery obtenerProductoRequisitoQuery = new ObtenerProductoRequisitoQuery();
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(productoRequisito);
            var handler = new ObtenerProductoRequisitosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProductoRequisitoQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.ProductoRequisito>>(result.Value);
        }

        [Fact]
        public async Task ObtenerProductoRequisitoPorId_Ok()
        {
            //Arange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito = 
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            //Act
            ObtenerProductoRequisitoPorIdQuery obtenerProductoRequisitoPorIdQuery = new ObtenerProductoRequisitoPorIdQuery();
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            var handler = new ObtenerProductoRequisitoPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProductoRequisitoPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductoRequisito_Ok()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito= productoRequisito, IdProductoRequisito= productoRequisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito( command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(productoRequisito);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ActualizarProductoRequisito_Duplicado()
        {
            //Arrange
            var productoRequisito = new Dominio.Entidades.ProductoRequisito()
                {
                    Id = 1,
                    IdRequisito = 1,
                    IdTipoProducto = 1
                };

            var listaCambios = new List<string> { "IdTipoProducto", "IdRequisito" };

            //Act
            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito = productoRequisito, IdProductoRequisito = productoRequisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(productoRequisito.IdTipoProducto)).ReturnsAsync(new Dominio.Entidades.TipoProducto());
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(productoRequisito.IdRequisito)).ReturnsAsync(new Dominio.Entidades.Requisito());
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ValidarProductoRequisitos(1, 1, 1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(productoRequisito);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductoRequisitoTipoProducto()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=2

                };

            var listaCambios = new List<string> { "IdTipoProducto" };

            //Act
            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito= productoRequisito, IdProductoRequisito= productoRequisito.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(productoRequisito);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarProductoRequisitoRequisito()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=2,
                    IdTipoProducto=1

                };

            var listaCambios = new List<string> { "IdRequisito" };

            //Act
            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito= productoRequisito, IdProductoRequisito= productoRequisito.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(productoRequisito);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarProductoRequisito_Error()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            var listaCambios = new List<string> { "IdTipoProducto " };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito = productoRequisito, IdProductoRequisito = productoRequisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(errorIsError);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarProductoRequisito_NoExiste()
        {
            //Arrange

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            var listaCambios = new List<string> { "IdTipoProducto" };
            var errorIsError = ErroresProductoRequisito.NoEncontrada;

            //Act

            EditarProductoRequisitoCommand command = new EditarProductoRequisitoCommand { ProductoRequisito = productoRequisito, IdProductoRequisito = productoRequisito.Id, ListaCambios= listaCambios };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ActualizarProductoRequisito(command.IdProductoRequisito, command.ListaCambios, command.ProductoRequisito)).ReturnsAsync(errorIsError);
            var handler = new EditarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", result.FirstError.Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProductoRequisito_Ok()
        {
            //Arrange
            var id = 1;

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            //Act
            EliminarProductoRequisitoCommand command = new EliminarProductoRequisitoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(id)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProductoRequisito_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresProductoRequisito.NoEncontrada;

            //Act
            EliminarProductoRequisitoCommand command = new EliminarProductoRequisitoCommand { Id = id};

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", result.FirstError.Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProductoRequisito_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresProductoRequisito.NoEncontrada;

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };

            //Act
            EliminarProductoRequisitoCommand command = new EliminarProductoRequisitoCommand { Id = id };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitoPorId(id)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("ProductoRequisito.NoEncontrada", result.FirstError.Code);
            Assert.Equal("ProductoRequisito configuracion no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProductoRequisito_Ok()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1
                };

            //Act

            CrearProductoRequisitoCommand command = new CrearProductoRequisitoCommand() { ProductoRequisito = productoRequisito };
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(productoRequisito)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            var handler = new CrearProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
        }
        [Fact]
        public async Task CrearProductoRequisito_ErrroDuplicado()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id = 1, Descripcion = "Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id = 1, Nombre = "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id = 1,
                    IdRequisito = 1,
                    IdTipoProducto = 1
                };

            //Act

            CrearProductoRequisitoCommand command = new CrearProductoRequisitoCommand() { ProductoRequisito = productoRequisito };
        
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ValidarProductoRequisitos(0, 1, 1)).ReturnsAsync(true);

            var handler = new CrearProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
          //  Assert.Equal(1, result.Value.Id);
        }
        [Fact]
        public async Task CrearProductoRequisitoTipoProductoError()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=2
                };

            //Act

            CrearProductoRequisitoCommand command = new CrearProductoRequisitoCommand() { ProductoRequisito = productoRequisito };
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(productoRequisito)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            var handler = new CrearProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearProductoRequisitoRequisitoError()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=2,
                    IdTipoProducto=1
                };

            //Act

            CrearProductoRequisitoCommand command = new CrearProductoRequisitoCommand() { ProductoRequisito = productoRequisito };
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(productoRequisito)).ReturnsAsync(productoRequisito);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            var handler = new CrearProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearProductoRequisito_Error()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion=1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id= 1, IdInstitucion=1, Nombre="Caracteristica 1" };
            var requisito = new Dominio.Entidades.Requisito { Id=1, Descripcion ="Requisito 1" };
            var categoria = new Dominio.Entidades.Categoria { Id=1, Nombre= "Categoria 1" };
            var productoRequisito =
                new Dominio.Entidades.ProductoRequisito()
                {
                    Id =1,
                    IdRequisito=1,
                    IdTipoProducto=1

                };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearProductoRequisitoCommand command = new CrearProductoRequisitoCommand() { ProductoRequisito = productoRequisito };
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductoPorId(1)).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(categoria);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(productoRequisito)).ReturnsAsync(errorIsError);

            var handler = new CrearProductoRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarDatosDatosProductoRequisitoError()
        {
            // Arrange
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };
  
            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            // Act
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

          
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosDatosRequisito_EliminarItemError()
        {
            // Arrange
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            // Act
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

         
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_DuplicadoenArchivoOK()
        {
            // Arrange
         
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1" }
            };
          
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);     
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

         
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_DuplicadoCategoriaArchivoOK()
        {
            // Arrange

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1",
                      
                },
                 new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1",

                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            // Act
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

         
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarError()
        {
            // Arrange
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            // Act
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

         
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarTipoError()
        {
            // Arrange
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };
            // Act
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

          
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
   
        [Fact]
        public async Task ImportarDatos_InsertDatos_OK()
        {
            // Arrange
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1", IdInstitucion = 5},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2", IdInstitucion = 5}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1", IdInstitucion = 5 }
            };
            var modo =1;
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_InsertDatos_ErrorTipoInstitucion()
        {
            // Arrange
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1", IdInstitucion = 5},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2", IdInstitucion = 5}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1", IdInstitucion = 7 }
            };
            var modo = 1;
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatos_Duplicado()
        {
            // Arrange
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1" }
            };
            var modo = 1;
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ValidarProductoRequisitos(0, 1, 1)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_ObtenerCategoria_Error()
        {
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo Producto 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 3" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1" }
            };
            var modo = 1;
                   
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);



            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatosRequisitoError()
        {
            // Arrange
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=10, Descripcion ="Requisito 1" }
            };
            var modo = 1;
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatosTipoProductoError()
        {
            // Arrange
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=10, Tipo="Tipo 10"},
                new Dominio.Entidades.TipoProducto { Id=10, Tipo="Tipo 20"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1" }
            };
            var modo = 1;
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatosError()
        {
            // Arrange
            var error = Error.Unexpected();
            var modo = 1;
            
            var datos = new List<ImportarProductoRequisitoCommandDto>
            {
                new ImportarProductoRequisitoCommandDto
                {
                     IdRequisito=1,Categoria="Categoria 1",TipoProducto="Tipo 1"
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var categorias = new List<Dominio.Entidades.Categoria>
            {
                new Dominio.Entidades.Categoria { Id=1, Nombre="Categoria 1" },
                new Dominio.Entidades.Categoria { Id=2, Nombre="Categoria 2" }

           };
            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1"},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2"}
            };
            var requisito = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito {Id=1, Descripcion ="Requisito 1" }
            };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.CrearProductoRequisito(It.IsAny<Dominio.Entidades.ProductoRequisito>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task BorradoMasivoRequisitos_DevuelveOk()
        {
            // Arrange
            var idsRequisitos = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarProductoRequisitosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductoRequisitosCommand { IdsRequisitos = idsRequisitos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task BorradoMasivoRequisitos_DevuelveError()
        {
            // Arrange
            var idsRequisitos = new List<int> { 1, 2, 3 };
            var erroror = ErroresProductoRequisito.NoEncontrada;

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.EliminarProductoRequisito(3)).ReturnsAsync(erroror);

            var handler = new EliminarProductoRequisitosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductoRequisitosCommand { IdsRequisitos = idsRequisitos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

















    }
}
