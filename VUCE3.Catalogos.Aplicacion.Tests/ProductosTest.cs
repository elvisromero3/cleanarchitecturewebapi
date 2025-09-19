using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductosPorId;
using VUCE3.Catalogos.Aplicacion.Productos.Queries.ObtenerProductos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EditarProductos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProducto;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.CrearProductos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Productos.Commands.EliminarProductos;


namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class ProductosTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public ProductosTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerProductos_Ok()
        {
            //Act
            var Productos = new List<Dominio.Entidades.Productos>()
            {
                new Dominio.Entidades.Productos()
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional=true

                }
            };

            ObtenerProductosQuery obtenerProductosQuery = new ObtenerProductosQuery();
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(Productos);
            var handler = new ObtenerProductosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProductosQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Productos>>(result.Value);
        }

        [Fact]
        public async Task ObtenerProductosPorId_Ok()
        {
            //Arange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            //Act
            ObtenerProductosPorIdQuery obtenerProductosPorIdQuery = new ObtenerProductosPorIdQuery();
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            var handler = new ObtenerProductosPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerProductosPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_Ok()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var listaCambios = new List<string> { "NombreComun", "NombreCientifico" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(Productos);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_Error()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            var listaCambios = new List<string> { "NombreComun" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(errorIsError);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarProductos_NoExiste()
        {
            //Arrange

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var listaCambios = new List<string> { "NombreComun" };
            var errorIsError = ErroresProductos.NoEncontrado;

            //Act

            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(errorIsError);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Producto no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProductos_Ok()
        {
            //Arrange
            var id = 1;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act
            EliminarProductoCommand command = new EliminarProductoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(id)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProductos_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresProductos.NoEncontrado;

            //Act
            EliminarProductoCommand command = new EliminarProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Producto no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarProductos_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresProductos.NoEncontrado;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act
            EliminarProductoCommand command = new EliminarProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(id)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.NoEncontrado", result.FirstError.Code);
            Assert.Equal("Producto no encontrado", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProductos_Ok()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(Productos);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Nombre Comun 1", result.Value.NombreComun);
        }

        [Fact]
        public async Task CrearProductos_ErrorNombre()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {

                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(Productos);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.NombreComunInvalido", result.FirstError.Code);
            Assert.Equal("El nombre común es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearProductos_Error()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(errorIsError);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }


        [Fact]
        public async Task EliminarProductos_DevuelveOk()
        {
            // Arrange

            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarProductoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductoCommand { Id = 1 }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarProductos_DevuelveError()
        {
            // Arrange
            var erroror = ErroresProductos.NoEncontrado;

            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(3)).ReturnsAsync(erroror);

            var handler = new EliminarProductoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductoCommand { Id = 1 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ValidarProductos_CrearDevuelveError()
        {
            // Arrange

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true

            };

            //Act           
            mockRepo.Setup(repo => repo.ProductosRepository.ValidarProductos(Productos.Id, Productos.NombreComun, Productos.NombreCientifico)).ReturnsAsync(true);
            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(Productos);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un Producto con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarProductos_EditarDevuelveExiste()
        {
            //Arrange

            var productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };
            var listaCambios = new List<string> { "NombreComun" };

            EditarProductosCommand command = new EditarProductosCommand { Productos = productos, IdProducto = productos.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ValidarProductos(productos.Id, productos.NombreComun, productos.NombreCientifico)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(productos);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Productos.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un Producto con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarProductos_EditarDevuelveError()
        {
            //Arrange

            var Productos = new Dominio.Entidades.Productos()
            {

                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };


            var listaCambios = new List<string> { "NombreComun" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ValidarProductos(Productos.Id, Productos.NombreComun, Productos.NombreCientifico)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(Productos);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProductos_DevuelveValidacionNombreError()
        {
            //Arrange
            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Productos 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var error = ErroresProductos.NombreComunInvalido;

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(error);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task CrearProductos_ClaseMayorA20()
        {
            //Arrange
            var error = ErroresProductos.TamanoClase;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1".PadRight(21, 'X'),
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(error);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProductos_PresentacionMayorA20()
        {
            //Arrange
            var error = ErroresProductos.TamanoPresentacion;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1".PadRight(21, 'X'),
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(error);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProductos_NombreComunMayorA50()
        {
            //Arrange
            var error = ErroresProductos.NombreComunInvalido;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1".PadRight(51, 'X'),
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(error);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearProductos_NombreCientificoMayorA50()
        {
            //Arrange
            var error = ErroresProductos.TamanoNombreCientifico;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1".PadRight(51, 'X'),
                Tradicional = true
            };

            //Act

            CrearProductosCommand command = new CrearProductosCommand() { Productos = Productos };
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(Productos)).ReturnsAsync(error);

            var handler = new CrearProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_ClaseMayorA20()
        {
            //Arrange
            var error = ErroresProductos.TamanoClase;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1".PadRight(21, 'X'),
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var listaCambios = new List<string> { "Clase" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(error);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_PresentacionMayorA20()
        {
            //Arrange
            var error = ErroresProductos.TamanoPresentacion;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1".PadRight(21, 'X'),
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var listaCambios = new List<string> { "Presentacion" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(error);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_NombreComunMayorA50()
        {
            //Arrange
            var error = ErroresProductos.NombreComunInvalido;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1".PadRight(51, 'X'),
                NombreCientifico = "Nombre Cientifico 1",
                Tradicional = true
            };

            var listaCambios = new List<string> { "NombreComun" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(error);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarProductos_NombreCientificoMayorA50()
        {
            //Arrange
            var error = ErroresProductos.TamanoNombreCientifico;

            var Productos = new Dominio.Entidades.Productos()
            {
                Id = 1,
                Clase = "Clase 1",
                Presentacion = "Presentacion 1",
                NombreComun = "Nombre Comun 1",
                NombreCientifico = "Nombre Cientifico 1".PadRight(51, 'X'),
                Tradicional = true
            };

            var listaCambios = new List<string> { "NombreCientifico" };

            //Act
            EditarProductosCommand command = new EditarProductosCommand { Productos = Productos, IdProducto = Productos.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(1)).ReturnsAsync(Productos);
            mockRepo.Setup(repo => repo.ProductosRepository.ActualizarProductos(command.IdProducto, command.ListaCambios, command.Productos)).ReturnsAsync(error);
            var handler = new EditarProductosCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveOk()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveError()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(ErroresProductos.NoEncontrado);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.NoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveErrorDuplicados()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                },

            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(ErroresProductos.DatosDuplicados);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.DatosDuplicados, result.FirstError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveErrorDuplicadosArchivo()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                },
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(ErroresProductos.DatosDuplicados);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.DatosDuplicadosArchivo, result.FirstError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveErrorExistenDuplicados()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                },
                new Dominio.Entidades.Productos
                {
                    Id = 2,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(ErroresProductos.DatosDuplicados);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.DatosDuplicadosArchivo, result.FirstError);
        }

        [Fact]
        public async Task ImportarProductos_DevuelveErrorObtenerEnEliminar()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(ErroresProductos.DatosDuplicados);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);            
        }

        [Fact]
        public async Task ImportarProductos_ErrorValidarProducto()
        {
            // Arrange
            var modo = 1;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ValidarProductos(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.DatosDuplicados, result.FirstError);
        }

        [Fact]
        public async Task ImportarProductos_NombreComunError()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "a".PadRight(51, 'a'),
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarProductos_NombreCientificoError()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "nombre",
                    NombreCientifico = "a".PadRight(51, 'a'),
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarProductos_PresentacionError()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "",
                    NombreComun = "Nombre común",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarProductos_ClaseError()
        {
            // Arrange
            var modo = 2;
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre común",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };

            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.ProductosRepository.CrearProductos(productos[0])).ReturnsAsync(productos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = productos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveOk()
        {
            // Arrange
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductos()).ReturnsAsync(productos);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProductosCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EliminarProductosCommand { Ids = new List<int> { 1 } }, default);
            
            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveErrorAlBorrar()
        {
            // Arrange
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(It.IsAny<int>())).ReturnsAsync(productos[0]);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(ErroresProductos.NoEncontrado);
            var handler = new EliminarProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductosCommand { Ids = new List<int> { 1 } }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.NoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task BorradoMasivo_DevuelveErrorAlBuscar()
        {
            // Arrange
            var productos = new List<Dominio.Entidades.Productos>
            {
                new Dominio.Entidades.Productos
                {
                    Id = 1,
                    Clase = "Clase 1",
                    Presentacion = "Presentacion 1",
                    NombreComun = "Nombre Comun 1",
                    NombreCientifico = "Nombre Cientifico 1",
                    Tradicional = true
                }
            };
            mockRepo.Setup(repo => repo.ProductosRepository.ObtenerProductoPorId(It.IsAny<int>())).ReturnsAsync(ErroresProductos.NoEncontrado);
            mockRepo.Setup(repo => repo.ProductosRepository.EliminarProductos(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            var handler = new EliminarProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarProductosCommand { Ids = new List<int> { 1 } }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresProductos.NoEncontrado, result.FirstError);
        }
    }
}
