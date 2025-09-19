using Moq;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EditarCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.CrearCategoria;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategoriaPorId;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategorias;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.EliminarCategoria;
using ErrorOr;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Aplicacion.Categoria.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Categoria.Queries.ObtenerCategorias;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CategoriaTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
        private Mock<IAccesoGestionUsuariosService> mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();

        public CategoriaTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockAccesoGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        }

        [Fact]
        public async Task ObtenerCategorias_Ok()
        {
            //Act
            var Categorias = new List<Dominio.Entidades.Categoria>()
            {
                new Dominio.Entidades.Categoria()
                {
                    Id = 1,
                   Nombre = "Categoria 1",
                   IdInstitucion = 5
                }
            };

            ObtenerCategoriasQuery obtenerCategoriaQuery = new ObtenerCategoriasQuery();
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(Categorias);
            var handler = new ObtenerCategoriasQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCategoriaQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.Categoria>>(result.Value);
        }

        [Fact]
        public async Task ObtenerCategoriaPorId_Ok()
        {
            //Arange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            //Act
            ObtenerCategoriaPorIdQuery obtenerPaisPorIdQuery = new ObtenerCategoriaPorIdQuery();
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            var handler = new ObtenerCategoriaPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerPaisPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearCategoria_Ok()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5

            };

            //Act

            CrearCategoriaCommand command = new CrearCategoriaCommand() { Categoria = Categoria };
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(Categoria)).ReturnsAsync(Categoria);

            var handler = new CrearCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal("Categoria 1", result.Value.Nombre);
        }

        [Fact]
        public async Task CrearCategoria_ErrorNombre()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = ""

            };

            //Act

            CrearCategoriaCommand command = new CrearCategoriaCommand() { Categoria = Categoria };
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(Categoria)).ReturnsAsync(Categoria);

            var handler = new CrearCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.NombreInvalido", result.FirstError.Code);
        }

        [Fact]
        public async Task CrearCategoria_Error()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearCategoriaCommand command = new CrearCategoriaCommand() { Categoria = Categoria };
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(Categoria)).ReturnsAsync(errorIsError);

            var handler = new CrearCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task CrearCategoria_DevuelveValidacionNombreError()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "C".PadLeft(101, 'y'),

            };

            var error = ErroresCategoria.NombreInvalido;

            //Act

            CrearCategoriaCommand command = new CrearCategoriaCommand() { Categoria = Categoria };
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(Categoria)).ReturnsAsync(error);

            var handler = new CrearCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ActualizarCategoria_Ok()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var listaCambios = new List<string> { "Nombre", "IdInstitucion" };

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(Categoria);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCategoria_InstitucionInvalidad()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 2
            };

            var listaCambios = new List<string> { "Nombre", "IdInstitucion" };

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(Categoria);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarCategoria_Error()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(errorIsError);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarCategoria_NoExiste()
        {
            //Arrange

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            var listaCambios = new List<string> { "Nombre" };
            var errorIsError = ErroresCategoria.NoEncontrada;

            //Act

            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(errorIsError);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Categoria no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarCategoria_DevuelveValidacionNombreError()
        {
            //Arrange
            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "C".PadLeft(101, 'y'),
            };

            var listaCambios = new List<string> { "Nombre" };

            var error = ErroresCategoria.NombreInvalido;

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(error);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task EliminarCategoria_Ok()
        {
            //Arrange
            var id = 1;

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"

            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1, IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            //Act
            EliminarCategoriaCommand command = new EliminarCategoriaCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(id)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(id)).ReturnsAsync(Result.Deleted);
            var handler = new Categoria.Commands.EliminarCategoria.EliminarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCategoria_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresCategoria.NoEncontrada;

            //Act
            EliminarCategoriaCommand command = new EliminarCategoriaCommand { Id = id };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(id)).ReturnsAsync(errorIsError);
            var handler = new Categoria.Commands.EliminarCategoria.EliminarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.NoEncontrada", result.FirstError.Code);
        }

        [Fact]
        public async Task EliminarCategoria_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresCategoria.NoEncontrada;

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            //Act
            EliminarCategoriaCommand command = new EliminarCategoriaCommand { Id = id };

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(id)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(id)).ReturnsAsync(errorIsError);
            var handler = new Categoria.Commands.EliminarCategoria.EliminarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Categoria no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task ValidarCategoria_EditarDevuelveExiste()
        {
            //Arrange

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 7
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(Categoria.Id, Categoria.Nombre, Categoria.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(Categoria);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.DatosDuplicados", result.FirstError.Code);
        }

        [Fact]
        public async Task ValidarCategoria_EditarDevuelveError()
        {
            //Arrange

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1"
            };


            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCategoriaCommand command = new EditarCategoriaCommand { Categoria = Categoria, IdCategoria = Categoria.Id, ListaCambios = listaCambios };
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategoriaPorId(1)).ReturnsAsync(Categoria);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(Categoria.Id, Categoria.Nombre, Categoria.IdInstitucion)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria)).ReturnsAsync(Categoria);
            var handler = new EditarCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ValidarCategoria_CrearDevuelveError()
        {
            // Arrange

            var Categoria = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5

            };

            //Act           
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(Categoria.Id, Categoria.Nombre, Categoria.IdInstitucion)).ReturnsAsync(true);
            CrearCategoriaCommand command = new CrearCategoriaCommand() { Categoria = Categoria };
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(Categoria)).ReturnsAsync(Categoria);

            var handler = new CrearCategoriaCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.DatosDuplicados", result.FirstError.Code);
        }
        
        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 1;

            List<InstitucionDto> instituciones = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                },
                new InstitucionDto()
                {
                    Id= 7,
                    Nombre = "DIPOA"
                }
            };


            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 1",
                    Institucion = "DCA",

                }
            };


            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 2",
                IdInstitucion = 5
            };


            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(1, "Categoria 1", 5)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]); 
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ErrorInstituciones()
        {
            // Arrange
            var modo = 1;

            List<InstitucionDto> instituciones = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                },
                new InstitucionDto()
                {
                    Id= 7,
                    Nombre = "DIPOA"
                }
            };


            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 1",
                    Institucion = "DCA",

                }
            };


            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 2",
                IdInstitucion = 5
            };


            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(1, "Categoria 1", 5)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_InstitucionInvalida()
        {
            // Arrange
            var modo = 1;

            List<InstitucionDto> instituciones = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                },
                new InstitucionDto()
                {
                    Id= 7,
                    Nombre = "DIPOA"
                }
                
            };


            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 1",
                    Institucion = "PROCOMER",

                }
            };


            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 2
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 2",
                IdInstitucion = 5
            };


            var institucion = new List<InstitucionDto>() { 
                new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                },
                new InstitucionDto()
                {
                    Id= 2,
                    Nombre = "PROCOMER"

                }
            };

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(1, "Categoria 1", 5)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]);
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
            var modo = 1;

            List<InstitucionDto> instituciones = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                },
                new InstitucionDto()
                {
                    Id= 7,
                    Nombre = "DIPOA"
                }
            };


            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "C".PadLeft(101, 'y'),
                    Institucion = "DCA",

                }
            };


            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 2",
                IdInstitucion = 5
            };


            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(1, "Categoria 1", 5)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicadosBD()
        {
            // Arrange
            var modo = 1;

            List<InstitucionDto> instituciones = new List<InstitucionDto>()
            { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                },
                new InstitucionDto()
                {
                    Id= 7,
                    Nombre = "DIPOA"
                }
            };


            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 1",
                    Institucion = "DCA",

                }
            };


            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 2,
                Nombre = "Categoria 2",
                IdInstitucion = 5
            };


            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(0, "Categoria 1", 5)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]);
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

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

            var datos = new List<ImportarCategoriaCommandDto>() {
                new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "Institucion 1",
                },
                new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "Institucion 1",
                }
            };
            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };

            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 1",
                IdInstitucion = 5
            };


            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);
            var error = ErroresCategoria.DatosDuplicados;
            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(categorias);
            mockRepo.Setup(repo => repo.CategoriaRepository.ValidarCategoria(categoria2.Id, categoria2.Nombre, categoria2.IdInstitucion)).ReturnsAsync(true);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "Institucion 1"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Categoria.DatosDuplicadosArchivo", result.FirstError.Code);
            Assert.Equal("Categoria contiene datos duplicados", result.FirstError.Description);
        }
        [Fact]
        public async Task ImportarDatos_ObtenerCategoriasDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "DCA",
                    IdInstitucion = 5
                }

            };

            var categoria1 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 01",
                IdInstitucion = 5,
            };


            var categoria2 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "Categoria 01",
                IdInstitucion = 5,
            };



            var categorias = new List<Dominio.Entidades.Categoria>();
            categorias.Add(categoria1);
            categorias.Add(categoria2);

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);


            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(categorias[0]);
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
        public async Task ImportarDatos_EliminarCategoriaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Categoria>() { new Dominio.Entidades.Categoria()
                {
                    Id = 1,
                    Nombre= "Categoria",
                    IdInstitucion = 5
                }
            };

            var datos1 = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "DCA",
                    IdInstitucion = 5
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };
            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos1 }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarCategoriaProductoRelacionado()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Categoria>() { new Dominio.Entidades.Categoria()
                {
                    Id = 4,
                    Nombre= "Categoria",
                    IdInstitucion = 5
                }
            };

            var datos1 = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "Institucion 1",
                }

            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos1 }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarCategoriaTipoProductoRelacionado()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Categoria>() { new Dominio.Entidades.Categoria()
                {
                    Id = 5,
                    Nombre= "Categoria",
                    IdInstitucion = 5
                }
            };

            var datos1 = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "DCA",
                }
            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"

                }
            };

            mockAccesoGestionUsuariosService.Setup(mockRepo => mockRepo.ObtenerInstituciones()).ReturnsAsync(institucion);

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos1 }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearCategoriaDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "Institucion 1",
                }

            };
            var datos1 = new List<Dominio.Entidades.Categoria>() { new Dominio.Entidades.Categoria()
            {
                Id = -1,
                Nombre= "Categoria",
                IdInstitucion= 5
            }
            };
            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);


            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos1);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(Error.Failure());

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 5,
                Nombre = "MINCEX"
            }
            };

            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

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
            var datos = new List<Dominio.Entidades.Categoria>() { new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre= "1234567890123456789012345678901234567890123456789012",
                IdInstitucion= 5
            }
            };

            var datos2 = new Dominio.Entidades.Categoria()
            {
                Id = 1,
                Nombre = "1234567890123456789012345678901234567890123456789012",
                IdInstitucion = 5
            };


            var datos1 = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "1234567890123456789012345678901234567890123456789012",
                    Institucion = "MINCEX",
                }

            };
            var error = ErroresCategoria.NombreInvalido;

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);



            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos);
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 5,
                Nombre = "MINCEX"
            }
            };



            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(error);

            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos1 }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(error.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task ImportarDatos_DatosDuplicados()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Categoria>()
            {
                new Dominio.Entidades.Categoria() { Nombre = "A",IdInstitucion =5 },
                new Dominio.Entidades.Categoria() { Nombre = "A" , IdInstitucion = 5}
            };

            var datos1 = new List<ImportarCategoriaCommandDto>() { new ImportarCategoriaCommandDto()
                {
                    Nombre = "Categoria 01",
                    Institucion = "Institucion 1",
                }

            };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);

            mockRepo.Setup(repo => repo.CategoriaRepository.ObtenerCategorias()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.CategoriaRepository.CrearCategoria(It.IsAny<Dominio.Entidades.Categoria>())).ReturnsAsync(datos[0]);

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
            {
                Id= 5,
                Nombre = "MINCEX"
            }
            };
            mockAccesoGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object, mockAccesoGestionUsuariosService.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos1 }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task EliminadoMasivoCategoria_DevuelveOk()
        {
            // Arrange
            var idsCategorias = new List<int> { 1, 2, 3 };

            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };



            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);


            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(2)).ReturnsAsync(It.IsAny<Deleted>());
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(3)).ReturnsAsync(It.IsAny<Deleted>());

            var handler = new Categoria.Commands.EliminarCategorias.EliminarCategoriaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCategoriasCommand { IdsCategorias = idsCategorias }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminadoMasivoCategorias_DevuelveError()
        {
            // Arrange
            var idsCategorias = new List<int> { 1, 2, 3 };
            var erroror = ErroresCategoria.NoEncontrada;
            var requisitos = new List<Dominio.Entidades.ProductoRequisito>
                {
                    new Dominio.Entidades.ProductoRequisito { Id = 1,IdRequisito=1,IdTipoProducto=1},
                    new Dominio.Entidades.ProductoRequisito { Id = 2,IdRequisito=2,IdTipoProducto=2}
                };

            var tipoproducto = new List<Dominio.Entidades.TipoProducto>
            {
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 1",IdCategoria=4},
                new Dominio.Entidades.TipoProducto { Id=1, Tipo="Tipo 2",IdCategoria=5}
            };

            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tipoproducto);


            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CategoriaRepository.EliminarCategoria(3)).ReturnsAsync(erroror);

            var handler = new Categoria.Commands.EliminarCategorias.EliminarCategoriaCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCategoriasCommand { IdsCategorias = idsCategorias }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }
    }
}
