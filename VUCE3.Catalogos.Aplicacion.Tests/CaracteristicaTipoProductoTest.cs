using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductosPorId;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Queries.ObtenerCaracteristicaTipoProductos;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EditarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProducto;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.EliminarCaracteristicaTipoProductos;
using VUCE3.Catalogos.Dominio.Errores;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.CrearCaracteristicaTipoProducto;

using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.CaracteristicaTipoProducto.Commands.ImportarDatos;
using VUCE3.Catalogos.Dominio.Constantes;
using System;
using VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias;
using System.Reflection.PortableExecutable;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class CaracteristicaTipoProductoTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();


        public CaracteristicaTipoProductoTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
        }
        [Fact]
        public async Task ObtenerCaracteristicaTipoProductos_Ok()
        {
            //Act
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto = new List<Dominio.Entidades.CaracteristicaTipoProducto>()
            {
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id =1,
                   TipoProducto = tipoproducto,
                   Caracteristica=caracteristica,
                }
            };

            ObtenerCaracteristicaTipoProductoQuery obtenerCaracteristicaTipoProductoQuery = new ObtenerCaracteristicaTipoProductoQuery();
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos()).ReturnsAsync(caracteristicaTipoProducto);
            var handler = new ObtenerCaracteristicaTipoProductosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCaracteristicaTipoProductoQuery, default);

            //Assert
            Assert.False(result.IsError);
            Assert.IsType<List<Dominio.Entidades.CaracteristicaTipoProducto>>(result.Value);
        }

        [Fact]
        public async Task ObtenerCaracteristicaTipoProductoPorId_Ok()
        {
            //Arange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            //Act
            ObtenerCaracteristicaTipoProductoPorIdQuery obtenerCaracteristicaTipoProductoPorIdQuery = new ObtenerCaracteristicaTipoProductoPorIdQuery();
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(1)).ReturnsAsync(caracteristicaTipoProducto);
            var handler = new ObtenerCaracteristicaTipoProductoPorIdQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerCaracteristicaTipoProductoPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarCaracteristicaTipoProducto_Ok()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCaracteristicaTipoProductoCommand command = new EditarCaracteristicaTipoProductoCommand { CaracteristicaTipoProducto = caracteristicaTipoProducto, IdCaracteristicaTipoProducto = caracteristicaTipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(1)).ReturnsAsync(caracteristicaTipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ActualizarCaracteristicaTipoProducto(command.IdCaracteristicaTipoProducto, command.ListaCambios, command.CaracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);
            var handler = new EditarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }
        [Fact]
        public async Task ActualizarCaracteristicaTipoProductoDuplicado()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            var listaCambios = new List<string> { "Nombre" };

            //Act
            EditarCaracteristicaTipoProductoCommand command = new EditarCaracteristicaTipoProductoCommand { CaracteristicaTipoProducto = caracteristicaTipoProducto, IdCaracteristicaTipoProducto = caracteristicaTipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(1)).ReturnsAsync(caracteristicaTipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ActualizarCaracteristicaTipoProducto(command.IdCaracteristicaTipoProducto, command.ListaCambios, command.CaracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);
            var handler = new EditarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarCaracteristicaTipoProducto_Error()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            var listaCambios = new List<string> { "IdTipoProducto " };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            EditarCaracteristicaTipoProductoCommand command = new EditarCaracteristicaTipoProductoCommand { CaracteristicaTipoProducto = caracteristicaTipoProducto, IdCaracteristicaTipoProducto = caracteristicaTipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(1)).ReturnsAsync(caracteristicaTipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ActualizarCaracteristicaTipoProducto(command.IdCaracteristicaTipoProducto, command.ListaCambios, command.CaracteristicaTipoProducto)).ReturnsAsync(errorIsError);
            var handler = new EditarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Unexpected", result.Errors[0].Code);
            Assert.Equal("An unexpected error has occurred.", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarCaracteristicaTipoProducto_NoExiste()
        {
            //Arrange

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            var listaCambios = new List<string> { "IdTipoProducto" };
            var errorIsError = ErroresCaracteristicaTipoProducto.NoEncontrada;

            //Act

            EditarCaracteristicaTipoProductoCommand command = new EditarCaracteristicaTipoProductoCommand { CaracteristicaTipoProducto = caracteristicaTipoProducto, IdCaracteristicaTipoProducto = caracteristicaTipoProducto.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(1)).ReturnsAsync(errorIsError);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ActualizarCaracteristicaTipoProducto(command.IdCaracteristicaTipoProducto, command.ListaCambios, command.CaracteristicaTipoProducto)).ReturnsAsync(errorIsError);
            var handler = new EditarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Característica tipo producto no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_Ok()
        {
            //Arrange
            var id = 1;

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            //Act
            EliminarCaracteristicaTipoProductoCommand command = new EliminarCaracteristicaTipoProductoCommand();
            command.Id = id;
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(id)).ReturnsAsync(caracteristicaTipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_NoExiste()
        {
            //Arrange
            var id = 1;
            var errorIsError = ErroresCaracteristicaTipoProducto.NoEncontrada;

            //Act
            EliminarCaracteristicaTipoProductoCommand command = new EliminarCaracteristicaTipoProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Característica tipo producto no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_Error()
        {
            //Arrange
            var id = -1;
            var errorIsError = ErroresCaracteristicaTipoProducto.NoEncontrada;

            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };

            //Act
            EliminarCaracteristicaTipoProductoCommand command = new EliminarCaracteristicaTipoProductoCommand { Id = id };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductoPorId(id)).ReturnsAsync(caracteristicaTipoProducto);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(id)).ReturnsAsync(errorIsError);
            var handler = new EliminarCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("CaracteristicaTipoProducto.NoEncontrada", result.FirstError.Code);
            Assert.Equal("Característica tipo producto no encontrada", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearCaracteristicaTipoProducto_Ok()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    IdCaracteristica = 1,
                    IdTipoProducto = 1
                };

            //Act

            CrearCaracteristicaTipoProductoCommand command = new CrearCaracteristicaTipoProductoCommand() { CaracteristicaTipoProducto = caracteristicaTipoProducto };
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new CrearCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.IdCaracteristica);
        }
        [Fact]
        public async Task CrearCaracteristicaTipoProductoDuplicado()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    IdCaracteristica = 1,
                    IdTipoProducto = 1
                };

            //Act

            CrearCaracteristicaTipoProductoCommand command = new CrearCaracteristicaTipoProductoCommand() { CaracteristicaTipoProducto = caracteristicaTipoProducto };
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new CrearCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task CrearCaracteristicaTipoProducto_Error()
        {
            //Arrange
            var tipoproducto = new Dominio.Entidades.TipoProducto { Id = 1, IdInstitucion = 1, IdCategoria = 1 };
            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, IdInstitucion = 1, Nombre = "Caracteristica 1" };
            var caracteristicaTipoProducto =
                new Dominio.Entidades.CaracteristicaTipoProducto()
                {
                    Id = 1,
                    TipoProducto = tipoproducto,
                    Caracteristica = caracteristica,

                };
            var errorIsError = ErrorOr.Error.Unexpected();

            //Act
            CrearCaracteristicaTipoProductoCommand command = new CrearCaracteristicaTipoProductoCommand() { CaracteristicaTipoProducto = caracteristicaTipoProducto };
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(errorIsError);

            var handler = new CrearCaracteristicaTipoProductoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_InsertDatosCaracteristicaError()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 3"
                }
            };

            var caracteristica = new Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1" },
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2" }
                };
            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_DuplicadoenArchivoOK()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                },
                new ImportarCaracteristicaTipoProductoCommandDto
{
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }};

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1" };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos()).ReturnsAsync(caracteristicaTipoProductos);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas())
                .ReturnsAsync(new List<Dominio.Entidades.Caracteristica> { caracteristica });

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto))
                .ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarError()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion = 5 };
            var tipoProducto = new Dominio.Entidades.TipoProducto { Id = 1, Tipo = "TipoProducto 1", IdInstitucion = 5 };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos()).ReturnsAsync(Error.Failure());

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos())
                .ReturnsAsync(new List<Dominio.Entidades.TipoProducto> { tipoProducto });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas())
                .ReturnsAsync(new List<Dominio.Entidades.Caracteristica> { caracteristica });

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto))
                .ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarTipoError()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion = 5 };
            var tipoProducto = new Dominio.Entidades.TipoProducto { Id = 1, Tipo = "TipoProducto 1", IdInstitucion = 5 };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos())
                .ReturnsAsync(new List<Dominio.Entidades.TipoProducto> { tipoProducto });

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas())
                .ReturnsAsync(new List<Dominio.Entidades.Caracteristica> { caracteristica });

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos()).ReturnsAsync(caracteristicaTipoProductos);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(1)).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosExistentesEliminarCreated()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion = 5 },
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2", IdInstitucion = 5 }
                };

            var caracteristica = new Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion= 5 };

            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };
            

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ObtenerCaracteristicaTipoProductos()).ReturnsAsync(caracteristicaTipoProductos);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(1)).ReturnsAsync(Result.Deleted);

            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatos_OK()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion = 5 },
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2", IdInstitucion = 5 }
                };

            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_InsertDatos_ErrorInstitucion()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1", IdInstitucion = 5 },
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2", IdInstitucion = 5 }
                };

            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 7 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_InsertDatosCaracteristicas_Error()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 3"
                }
            };

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1" , IdInstitucion = 5},
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2" , IdInstitucion = 5}
                };
            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(caracteristicaTipoProducto)).ReturnsAsync(caracteristicaTipoProducto);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatosValidadCaracteristicaTipoExiste()
        {
            // Arrange
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1" , IdInstitucion = 5},
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2" , IdInstitucion = 5}
                };
            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task ImportarDatos_InsertDatosError()
        {
            // Arrange
            var error = Error.Unexpected();
            var caracteristicaTipoProductos = new List<Dominio.Entidades.CaracteristicaTipoProducto>
            {
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 1, IdCaracteristica=1, IdTipoProducto=1 },
                new Dominio.Entidades.CaracteristicaTipoProducto { Id = 2, IdCaracteristica=2, IdTipoProducto=2 }
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id = 1,
                   IdCaracteristica = 1,
                   IdTipoProducto = 1

               };
            var modo = 1;
            var datos = new List<ImportarCaracteristicaTipoProductoCommandDto>
            {
                new ImportarCaracteristicaTipoProductoCommandDto
                {
                    IdTipoProducto = 1,
                    Caracteristica = "Caracteristica 1"
                }
            };

            var caracteristica = new Dominio.Entidades.Caracteristica { Id = 1, Nombre = "Caracteristica 1" };
            var caracteristicas = new List<Caracteristica>
                {
                    new Caracteristica { Id = 1, Nombre = "Caracteristica 1" , IdInstitucion = 5},
                    new Caracteristica { Id = 2, Nombre = "Caracteristica 2" , IdInstitucion = 5}
                };
            var tiposProductos = new List<Dominio.Entidades.TipoProducto>
                {
                    new Dominio.Entidades.TipoProducto { Id = 1, IdCategoria = 1, Tipo= "tipo1", IdInstitucion = 5 },
                };

            mockRepo.Setup(repo => repo.TipoProductoRepository.ObtenerTipoProductos()).ReturnsAsync(tiposProductos);
            mockRepo.Setup(repo => repo.CaracteristicasRepository.ObtenerCaracteristicas()).ReturnsAsync(caracteristicas);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.ValidarCaracteristicaTipoProducto(1, 1)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.CrearCaracteristicaTipoProducto(It.IsAny<Dominio.Entidades.CaracteristicaTipoProducto>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_DevuelveOk()
        {
            // Arrange
            var idsCaracteristicaTipoProducto = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarCaracteristicaTipoProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCaracteristicaTipoProductosCommand { IdsCaracteristicaTipoProductos = idsCaracteristicaTipoProducto }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarCaracteristicaTipoProducto_DevuelveError()
        {
            // Arrange
            var idsCaracteristicaTipoProducto = new List<int> { 1, 2, 3 };
            var erroror = ErroresCaracteristicaTipoProducto.NoEncontrada;

            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.CaracteristicaTipoProductoRepository.EliminarCaracteristicaTipoProducto(3)).ReturnsAsync(erroror);

            var handler = new EliminarCaracteristicaTipoProductosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarCaracteristicaTipoProductosCommand { IdsCaracteristicaTipoProductos = idsCaracteristicaTipoProducto }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }










    }
}
