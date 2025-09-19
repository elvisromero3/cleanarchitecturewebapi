using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.CrearRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EditarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisito;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisitos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitoPorId;
using VUCE3.Catalogos.Aplicacion.Requisitos.Queries.ObtenerRequisitos;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Aplicacion.Servicios.DTO;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class RequisitosTest
    {
        private Mock<IUnitOfWork> mockRepo;
        private Mock<IAccesoGestionUsuariosService> mockGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();

        public RequisitosTest()
        {
            mockRepo = new Mock<IUnitOfWork>();
            mockGestionUsuariosService = new Mock<IAccesoGestionUsuariosService>();
        }

        [Fact]
        public async Task ObtenerRequisitos_Ok()
        {
            //Arange
            var lstRequisitos = new List<Requisito>()
            {
                new Requisito{
                    Id =1,
                    Codigo = "01",
                    Descripcion = "Descripción 1",
                    Version = "Version 1",
                    IdPais = 1,
                    Activo = true,
                    IdInstitucion = 1
                },
                new Requisito{
                    Id = 2,                    
                    Codigo = "02",
                    Descripcion = "Descripción 2",
                    Version = "Version 1",
                    IdPais = 1,
                    Activo = true,
                    IdInstitucion = 1
                },
            };

            //Act
            ObtenerRequisitosQuery obtenerRequisitosQuery = new ObtenerRequisitosQuery();
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(lstRequisitos);
            var handler = new ObtenerRequisitosQueryHandler(mockRepo.Object);
            var result = await handler.Handle(obtenerRequisitosQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ObtenerRequisitoPorId_Ok()
        {
            //Arange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            ObtenerRequisitoPorIdQuery obtenerRequisitoPorIdQuery = new ObtenerRequisitoPorIdQuery();
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            var handler = new ObtenerRequisitoPorIdQueryhandler(mockRepo.Object);
            var result = await handler.Handle(obtenerRequisitoPorIdQuery, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task CrearRequisito_Ok()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",                
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Descripción 1", result.Value.Descripcion);
        }

        [Fact]
        public async Task CrearRequisitoCodigoError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "0123456789012345678912365478965412365478965412301236544789999",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
        
        }
        [Fact]
        public async Task CrearRequisitoVersionError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 123",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
        }
        [Fact]
        public async Task CrearRequisitoInstitucionError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 3,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 3,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);

        }
        [Fact]
        public async Task CrearRequisito_ErrorDescripcion()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Requisito.DescripcionInvalida", result.Errors[0].Code);
            Assert.Equal("La descripción es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorObtenerPais()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(ErroresPaises.NoEncontrado);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Pais.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("País no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorInstituciones()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.FalloServicioAccesoGestionUsuario", result.Errors[0].Code);
            Assert.Equal("Falló al obtener instituciones", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorObtenerInstitucion()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 2,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.IntitucionNoEncontrada", result.Errors[0].Code);
            Assert.Equal("Institución no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearRequisito_ExisteDuplicado()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.DatosDuplicados", result.Errors[0].Code);
            Assert.Equal("Ya existe un requisito con los datos proporcionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorCrearRequisito()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(Error.Failure());
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorNoExisteImagen()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "",
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Requisito.RequisitoImagenObligatoria", result.FirstError.Code);
            Assert.Equal("La imagen es un campo requerido, debe tener un tamaño mayor a 0", result.FirstError.Description);
        }

        [Fact]
        public async Task CrearRequisito_ErrorNombreImagen()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = ""
            };

            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            //Act
            CrearRequisitoCommand command = new CrearRequisitoCommand() { Requisito = requisito };
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(requisito.Id, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);
            var handler = new CrearRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.IsError);
            Assert.Equal("Requisito.RequisitoNombreImagen", result.FirstError.Code);
            Assert.Equal("El nombre de la imagen es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarRequisito_Ok()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };

            var productosRequisito = new List<Dominio.Entidades.ProductoRequisito>() { new Dominio.Entidades.ProductoRequisito
                {
                    Id = 1,
                    IdRequisito = 2,
                }
            };

            var listaCambios = new List<string> { "Codigo", "Version", "IdPais", "IdInstitucion" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(productosRequisito);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task ActualizarRequisitoCodigoError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "012345678901234567890123456789987456321014785236523",
                Descripcion = "Descripción 123Descripción 123Descripción 123Descripción 123",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            

            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarRequisitoProductosAsociadosError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "1",
                Descripcion = "a",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 2,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var requisitoNuevo = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var productosRequisito = new List<Dominio.Entidades.ProductoRequisito>() { new Dominio.Entidades.ProductoRequisito
                {
                    Id = 1,
                    IdRequisito = 1,
                } 
            };
            var listaCambios = new List<string> { "IdInstitucion" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisitoNuevo, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.ProductoRequisitoRepository.ObtenerProductoRequisitos()).ReturnsAsync(productosRequisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarRequisitoVersionError()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 12345",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarRequisitoInstitucionInvalida()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 12345",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var requisitoNuevo = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 4,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "DCA"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "IdInstitucion" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisitoNuevo, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ActualizarRequisito_ErrorRequisitoNoExiste()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(ErroresRequisito.RequisitoNoEncontrado);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Requisito no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorDescripcion()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Descripcion", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.DescripcionInvalida", result.Errors[0].Code);
            Assert.Equal("La descripción es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorIdPais()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "IdPais", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(ErroresPaises.NoEncontrado);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Pais.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("País no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorInstituciones()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var requisitoNuevo = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 5,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "IdInstitucion", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisitoNuevo, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.FalloServicioAccesoGestionUsuario", result.FirstError.Code);
            Assert.Equal("Falló al obtener instituciones", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorIdInstitucion()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 2,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var requisitoNuevo = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 4,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 5,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "IdInstitucion", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisitoNuevo, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.IntitucionNoEncontrada", result.Errors[0].Code);
            Assert.Equal("Institución no encontrada", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorDatosDuplicados()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ExisteDuplicado()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(true);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.DatosDuplicados", result.Errors[0].Code);
            Assert.Equal("Ya existe un requisito con los datos proporcionados", result.Errors[0].Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorActualizarRequisito()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = "NombreImagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "Codigo", "Version" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(Error.Failure());
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorExistenciaImagen()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "",
                NombreImagenRequisito = "Imagen.pdf"
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "ImagenRequisito" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ActualizarRequisito_ErrorNombreImagen()
        {
            //Arrange
            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1,
                ImagenRequisito = "Imagen",
                NombreImagenRequisito = ""
            };
            var institucion = new List<InstitucionDto>() { new InstitucionDto()
                {
                    Id= 1,
                    Nombre = "ANAQ"
                }
            };

            var pais = new Dominio.Entidades.Pais
            {
                Id = 1,
                Nombre = "Costa Rica",
            };
            var listaCambios = new List<string> { "NombreImagenRequisito" };

            //Act
            EditarRequisitoCommand command = new EditarRequisitoCommand() { Requisito = requisito, IdRequisito = requisito.Id, ListaCambios = listaCambios };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaisPorId(1)).ReturnsAsync(pais);
            mockGestionUsuariosService.Setup(repo => repo.ObtenerInstituciones()).ReturnsAsync(institucion);
            mockRepo.Setup(repo => repo.RequisitosRepository.ValidarRequisito(1, requisito.Codigo, requisito.Version, requisito.IdPais, requisito.IdInstitucion)).ReturnsAsync(false);
            mockRepo.Setup(repo => repo.RequisitosRepository.ActualizarRequisito(command.Requisito, command.IdRequisito, command.ListaCambios)).ReturnsAsync(requisito);
            var handler = new EditarRequisitoCommandHandler(mockRepo.Object, mockGestionUsuariosService.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task EliminarRequisito_Ok()
        {
            //Arrange
            var id = 1;

            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            EliminarRequisitoCommand command = new EliminarRequisitoCommand() { Id = id }; 
            
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarRequisito_ErrorObtenerRequisito()
        {
            //Arrange
            var id = 1;

            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            EliminarRequisitoCommand command = new EliminarRequisitoCommand() { Id = id };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(ErroresRequisito.RequisitoNoEncontrado);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(id)).ReturnsAsync(Result.Deleted);
            var handler = new EliminarRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("Requisito.NoEncontrado", result.Errors[0].Code);
            Assert.Equal("Requisito no encontrado", result.Errors[0].Description);
        }

        [Fact]
        public async Task EliminarRequisito_ErrorEliminar()
        {
            //Arrange
            var id = 1;

            var requisito = new Requisito
            {
                Id = 1,
                Codigo = "01",
                Descripcion = "Descripción 1",
                Version = "Version 1",
                IdPais = 1,
                Activo = true,
                IdInstitucion = 1
            };

            //Act
            EliminarRequisitoCommand command = new EliminarRequisitoCommand() { Id = id };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitoPorId(1)).ReturnsAsync(requisito);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(id)).ReturnsAsync(Error.Failure());
            var handler = new EliminarRequisitoCommandHandler(mockRepo.Object);
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.IsError);
            Assert.Equal("General.Failure", result.FirstError.Code);
            Assert.Equal("A failure has occurred.", result.FirstError.Description);
        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosDatosRequisitoError()
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
                   Id =1,
                   IdCaracteristica=1,
                   IdTipoProducto=1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }
            };

            var requisito = new Dominio.Entidades.Requisito
            {
                Id = 1,
                Codigo="01",
                Descripcion ="Descripcion 1",
                Version ="Version 1",
                Activo= true,
                IdInstitucion=1,
                IdPais=1
            };
            var requisitos = new List<Requisito>
                {
                    new Requisito {Id = 1, Codigo="01",Descripcion ="Descripcion 1", Version ="Version 1", Activo= true , IdInstitucion=1 , IdPais=1 },
                    new Requisito {Id = 1, Codigo="01",Descripcion ="Descripcion 1", Version ="Version 1", Activo= true , IdInstitucion=1 , IdPais=1 }
                };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_EliminarDatosDatosRequisito_EliminarItemError()
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
                   Id =1,
                   IdCaracteristica=1,
                   IdTipoProducto=1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }
            };

            var requisito = new Dominio.Entidades.Requisito
            {
                Id = 1,
                Codigo="01",
                Descripcion ="Descripcion 1",
                Version ="Version 1",
                Activo= true,
                IdInstitucion=1,
                IdPais=1
            };
            var requisitos = new List<Requisito>
                {
                    new Requisito {Id = 1, Codigo="01",Descripcion ="Descripcion 1", Version ="Version 1", Activo= true , IdInstitucion=1 , IdPais=1 },
                    new Requisito {Id = 1, Codigo="01",Descripcion ="Descripcion 1", Version ="Version 1", Activo= true , IdInstitucion=1 , IdPais=1 }
                };

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(Error.Failure());
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisito)).ReturnsAsync(requisito);

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
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito
                {
                    Id = 1,
                    Codigo="01",
                    Descripcion="Requisito 1",
                    Version ="Version 1",
                    Activo= true,
                    IdInstitucion=1,
                    IdPais=1
                },
                new Dominio.Entidades.Requisito
                { 
                    Id = 2,
                    Codigo="02",
                    Descripcion="Requisito 2",
                    Version ="Version 1",
                    Activo= true,
                    IdInstitucion=1,
                    IdPais=1}
            };

            var caracteristicaTipoProducto =
               new Dominio.Entidades.CaracteristicaTipoProducto()
               {
                   Id =1,
                   IdCaracteristica=1,
                   IdTipoProducto=1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }};


            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(Result.Deleted);           

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
                   Id =1,
                   IdCaracteristica=1,
                   IdTipoProducto=1

               };
            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }};

 

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(Error.Failure());

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
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Dominio.Entidades.Requisito
                {
                    Id = 1,
                    Codigo="01",
                    Descripcion="Requisito 1",
                    Version ="Version 1",
                    Activo= true,
                    IdInstitucion=1,
                    IdPais=1
                },
                new Dominio.Entidades.Requisito
                {
                    Id = 2,
                    Codigo="02",
                    Descripcion="Requisito 2",
                    Version ="Version 1",
                    Activo= true,
                    IdInstitucion=1,
                    IdPais=1}
            };

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                    Descripcion ="",
                    IdInstitucion =1,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }};

      

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(Error.Failure());

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
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=5,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=5,IdPais=1}
            };

            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };

            var modo = ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                }};

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);
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
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=1,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=1,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01",
                     Descripcion ="Requisito 1",
                     IdInstitucion =5,
                     Version ="Version 1",
                     Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02",
                    Descripcion ="Requisito 2",
                    IdInstitucion =5,
                    Version ="Version 1",
                    Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

        
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_ObtenerPais_Error()
        {
            // Arrange
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=1,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=1,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =1,Version ="Version 1",Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =1,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Guatemala"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act

            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }


        [Fact]
        public async Task ImportarDatos_InsertDatosErrorInstitucion()
        {
            // Arrange
            var error = Error.Unexpected();
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=4,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=4,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =4,Version ="Version 1",Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =4,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(error);

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
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=5,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=5,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                    Codigo ="01", Descripcion ="Requisito 1",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(It.IsAny<Requisito>())).ReturnsAsync(error);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);

        }
        [Fact]
        public async Task ImportarDatos_InsertDatosDescripcion()
        {
            // Arrange
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=1,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=1,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                     Codigo ="01",
                     Descripcion ="Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1 Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1 Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1Requisito 1",
                     IdInstitucion =5,
                     Version ="Version 1",
                     Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_InsertDatosCodigo()
        {
            // Arrange
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=1,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=1,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                     Codigo ="0123456789012345678912365478965412365478965412301236544789999",
                     Descripcion ="Requisito 1",
                     IdInstitucion =5,
                     Version ="Version 1",
                     Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);


            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }
        [Fact]
        public async Task ImportarDatos_InsertDatosVersion()
        {
            // Arrange
            var requisitos = new List<Dominio.Entidades.Requisito>
            {
                new Requisito
                {
                    Id = 1, Codigo="01",Descripcion="Requisito 1", Version ="Version 1",Activo= true,IdInstitucion=1,IdPais=1
                },
                new Requisito
                {
                    Id = 2, Codigo="02",Descripcion="Requisito 2", Version ="Version 1", Activo= true,IdInstitucion=1,IdPais=1}
            };

            var modo = 1;
            var datos = new List<ImportarRequisitoCommandDto>
            {
                 new ImportarRequisitoCommandDto
                {
                     Codigo ="01",
                     Descripcion ="Requisito 1",
                     IdInstitucion =5,
                     Version ="Version 1234",
                     Pais="Costa Rica"
                },
                new ImportarRequisitoCommandDto
                {
                    Codigo ="02", Descripcion ="Requisito 2",IdInstitucion =5,Version ="Version 1",Pais="Costa Rica"
                }};
            var paises = new List<Dominio.Entidades.Pais>
            {
                new Dominio.Entidades.Pais
                {
                   Id=1,
                   Nombre="Costa Rica"
                },
                new Dominio.Entidades.Pais
                {
                    Id=2,
                    Nombre="Panama"
                }

           };
            // Act
            mockRepo.Setup(repo => repo.RequisitosRepository.ObtenerRequisitos()).ReturnsAsync(requisitos);
            mockRepo.Setup(repo => repo.PaisesRepository.ObtenerPaises()).ReturnsAsync(paises);
            mockRepo.Setup(repo => repo.RequisitosRepository.CrearRequisito(requisitos[0])).ReturnsAsync(requisitos[0]);

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

            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarRequisitosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarRequisitosCommand { IdsRequisitos = idsRequisitos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task BorradoMasivoRequisitos_DevuelveError()
        {
            // Arrange
            var idsRequisitos = new List<int> { 1, 2, 3 };
            var erroror = ErroresRequisito.RequisitoNoEncontrado;

            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.RequisitosRepository.EliminarRequisito(3)).ReturnsAsync(erroror);

            var handler = new EliminarRequisitosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarRequisitosCommand { IdsRequisitos = idsRequisitos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }





    }
}
