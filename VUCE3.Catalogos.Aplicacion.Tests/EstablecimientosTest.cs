using ErrorOr;
using Moq;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.CrearEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EditarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.EliminarEstablecimiento;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Commands.ImportarDatos;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientoPorId;
using VUCE3.Catalogos.Aplicacion.Establecimiento.Queries.ObtenerEstablecimientos;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;
using System.Reflection.Metadata;

namespace VUCE3.Catalogos.Aplicacion.Tests
{
    public class EstablecimientosTest
    {
        private Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();

        // Pruebas de Lectura de Establecimiento

        [Fact]
        public async Task ObtenerEstablecimientos_Ok()
        {
            // Arrange
            var establecimientos = new List<Dominio.Entidades.Establecimiento>
            {
                new Dominio.Entidades.Establecimiento
                {
                    Id = 1,
                    NumeroCvo = "123456",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "Activo",
                  
                },
                new Dominio.Entidades.Establecimiento
                {
                    Id = 1,
                    NumeroCvo = "654321",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(7),
                     EstadoEstablecimiento= "Activo",
                }
            };

            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(establecimientos);

            var handler = new ObtenerEstablecimientosQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerEstablecimientosQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(establecimientos, result.Value);
        }

        [Fact]
        public async Task ObtenerEstablecimientosPorId_Ok()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };

            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(1)).ReturnsAsync(establecimiento);

            var handler = new ObtenerEstablecimientosPorIdQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ObtenerEstablecimientosPorIdQuery { IdEstablecimiento = 1 }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(establecimiento, result.Value);
        }

        [Fact]
        public async Task ObtenerEstablecimientosPorId_Error()
        {
            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(1)).ReturnsAsync(ErroresEstablecimiento.NoEncontrado);
            var handler = new ObtenerEstablecimientosPorIdQueryHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ObtenerEstablecimientosPorIdQuery { IdEstablecimiento = 1 }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, result.FirstError);
        }

        // Pruebas de Creacion de Establecimiento

        [Fact]
        public async Task CrearEstablecimiento_Ok()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(establecimiento);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.False(result.IsError);
            Assert.Equal(establecimiento, result.Value);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorNumeroCvoVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.NumeroCvoInvalido);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.NumeroCvoInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorActividadPrimariaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.ActividadPrimariaInvalido);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ActividadPrimariaInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorActividadSecundariaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "difbgvadnvxlkgnaoévladknvoavlanvokanervoulnXLMNcvoáLSVNsobvkc vaóubvNvóaiwv,mx vao´vn váonf",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.ActividadSecundariaInvalido);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ActividadSecundariaInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorProvinciaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.ProvinciaInvalido);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ProvinciaInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorCantonVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.CantonInvalido);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.CantonInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorFechaVencimientoMenorHoy()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(-5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.FechaVencimientoMenorHoy);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.FechaVencimientoMenorHoy, result.FirstError);
        }
        [Fact]
        public async Task CrearEstablecimiento_ErrorEstaDoEstablecimiento()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "ActivoQWERTYUIOPLKJHG",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.FechaVencimientoMenorHoy);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.EstadoEstablecimientoInvalido, result.FirstError);
        }

        [Fact]
        public async Task CrearEstablecimiento_ErrorEstablecimientoExiste()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };
            mockRepo.Setup(x => x.EstablecimientosRepository.ValidarEstablecimiento(establecimiento.Id, establecimiento.NumeroCvo)).ReturnsAsync(true);
            mockRepo.Setup(x => x.EstablecimientosRepository.CrearEstablecimiento(establecimiento)).ReturnsAsync(ErroresEstablecimiento.EstablecimientoExiste);
            var handler = new CrearEstablecimientoCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new CrearEstablecimientoCommand { Establecimiento = establecimiento }, CancellationToken.None);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.EstablecimientoExiste, result.FirstError);
        }

        // Pruebas de Edicion de Establecimiento

        [Fact]
        public async Task EditarEstablecimiento_Ok()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                NombreEstablecimiento = "Npmbre establecimiento 1",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                Distrito = "Distrito1",
                DireccionExacta = "351 Calle Las Rosas",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };

            List<string> listaCambios = new List<string> { "NumeroCvo", "ActividadPrimaria", "ActividadSecundaria", "Provincia", "Canton", "FechaVencimiento" };
            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(establecimiento.Id)).ReturnsAsync(establecimiento);
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(establecimiento);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.False(result.IsError);
            Assert.Equal(establecimiento, result.Value.Item2);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorNoEncontrado()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
            };

            List<string> listaCambios = new List<string> { "NumeroCvo" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(establecimiento.Id)).ReturnsAsync(ErroresEstablecimiento.NoEncontrado);
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(establecimiento);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorNumeroCvoVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "",
            };

            List<string> listaCambios = new List<string> { "NumeroCvo" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(ErroresEstablecimiento.NumeroCvoInvalido);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.NumeroCvoInvalido, result.FirstError);
        }
        [Fact]
        public async Task EditarEstablecimiento_ErrorNumeroCvoDuplicado()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "test",
            };

            List<string> listaCambios = new List<string> { "NumeroCvo" };
            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(establecimiento.Id)).ReturnsAsync(establecimiento);
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(ErroresEstablecimiento.NumeroCvoInvalido);
            mockRepo.Setup(x => x.EstablecimientosRepository.ValidarEstablecimiento(establecimiento.Id, establecimiento.NumeroCvo)).ReturnsAsync(true);
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
           // Assert.Equal(ErroresEstablecimiento.EstablecimientoExiste, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorActividadPrimariaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                ActividadPrimaria = "",
            };

            List<string> listaCambios = new List<string> { "ActividadPrimaria" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(ErroresEstablecimiento.ActividadPrimariaInvalido);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ActividadPrimariaInvalido, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorActividadSecundariaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                ActividadSecundaria = new string('a',200),
            };

            List<string> listaCambios = new List<string> { "ActividadSecundaria" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento))
                .ReturnsAsync(ErroresEstablecimiento.ActividadSecundariaInvalido);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ActividadSecundariaInvalido, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorProvinciaVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                Provincia = "",
            };

            List<string> listaCambios = new List<string> { "Provincia" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento))
                .ReturnsAsync(ErroresEstablecimiento.ProvinciaInvalido);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.ProvinciaInvalido, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorCantonVacio()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                Canton = "",
            };

            List<string> listaCambios = new List<string> { "Canton" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento))
                .ReturnsAsync(ErroresEstablecimiento.CantonInvalido);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.CantonInvalido, result.FirstError);
        }

        [Fact]
        public async Task EditarEstablecimiento_ErrorFechaVencimientoMenorHoy()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                FechaVencimiento = DateTime.Now.AddDays(-5),
            };

            List<string> listaCambios = new List<string> { "FechaVencimiento" };
            
            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(ErroresEstablecimiento.FechaVencimientoMenorHoy);
            
            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);
            
            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);
            
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.FechaVencimientoMenorHoy, result.FirstError);
        }
        [Fact]
        public async Task EditarEstablecimiento_ErrorEstadoEstablecimiento()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                EstadoEstablecimiento = "Inactivo12345678909876",
            };

            List<string> listaCambios = new List<string> { "EstadoEstablecimiento" };

            mockRepo.Setup(x => x.EstablecimientosRepository.ActualizarEstablecimiento(establecimiento.Id, listaCambios, establecimiento)).ReturnsAsync(ErroresEstablecimiento.EstadoEstablecimientoInvalido);

            var handler = new EditarEstablecimientoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EditarEstablecimientoCommand { Establecimiento = establecimiento, IdEstablecimiento = establecimiento.Id, ListaCambios = listaCambios }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.EstadoEstablecimientoInvalido, result.FirstError);
        }
        //EliminarEstablecimiento

        [Fact]
        public async Task EliminarEstablecimiento_Ok()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
                NumeroCvo = "123456",
                ActividadPrimaria = "Actividad1",
                ActividadSecundaria = "Actividad2",
                Provincia = "Provincia1",
                Canton = "Canton1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento = "Activo",
            };

            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(establecimiento.Id)).ReturnsAsync(establecimiento);
            mockRepo.Setup(x => x.EstablecimientosRepository.EliminarEstablecimiento(establecimiento.Id)).ReturnsAsync(Result.Deleted);

            var handler = new EliminarEstablecimientoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEstablecimientoCommand { Id = establecimiento.Id }, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarEstablecimiento_ErrorNoEncontrado()
        {
            var establecimiento = new Dominio.Entidades.Establecimiento
            {
                Id = 1,
            };

            mockRepo.Setup(x => x.EstablecimientosRepository.ObtenerEstablecimientoPorId(establecimiento.Id))
                .ReturnsAsync(ErroresEstablecimiento.NoEncontrado);
            mockRepo.Setup(x => x.EstablecimientosRepository.EliminarEstablecimiento(establecimiento.Id))
                .ReturnsAsync(ErroresEstablecimiento.NoEncontrado);

            var handler = new EliminarEstablecimientoCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEstablecimientoCommand { Id = establecimiento.Id }, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(ErroresEstablecimiento.NoEncontrado, result.FirstError);
        }

        [Fact]
        public async Task EliminarEstablecimientos_DevuelveOk()
        {
            // Arrange
            var idsEstablecimientos = new List<int> { 1, 2, 3 };

            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(1)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(2)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(3)).ReturnsAsync(It.IsAny<ErrorOr.Deleted>());

            var handler = new EliminarEstablecimientosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEstablecimientosCommand { IdsEstablecimientos = idsEstablecimientos }, default);

            // Assert
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task EliminarEstablecimientos_DevuelveError()
        {
            // Arrange
            var idsEstablecimientos = new List<int> { 1, 2, 3 };
            var erroror = ErroresCultivo.NoEncontrado;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(1)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(2)).ReturnsAsync(erroror);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(3)).ReturnsAsync(erroror);

            var handler = new EliminarEstablecimientosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new EliminarEstablecimientosCommand { IdsEstablecimientos = idsEstablecimientos }, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(erroror, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_ValidarEstablecimientosDuplicados()
        {
            // Arrange
            var modo = 1;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    Id= 1,
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "Activo",}

            };

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.Equal("Establecimiento.DatosDuplicados", result.FirstError.Code);
            Assert.Equal("Ya existe un establecimiento con los datos proporcionados", result.FirstError.Description);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNumeroCvError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456789012345678901234567890123456789012345678901234567890",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                   EstadoEstablecimiento= "Activo",} };

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var error = ErroresEstablecimiento.NumeroCvoInvalido;

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionNombreEstablecimientoError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "123456789012345678901234567890123456789012345678901234567890",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.NombreEstablecimientoInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id,  datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionActividadPrimariaError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "123456789012345678901234567890123456789012345678901234567890",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.ActividadPrimariaInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionActividadSecundariaError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "123456789012345678901234567890123456789012345678901234567890",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.ActividadSecundariaInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionProvinciaError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "123456789012345678901234567890123456789012345678901234567890",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                   EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.ProvinciaInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionCantonError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "123456789012345678901234567890123456789012345678901234567890",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                   EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.CantonInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionDistritoError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "123456789012345678901234567890123456789012345678901234567890",
                    DireccionExacta = "Direccion 1",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "rttttt",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                  EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.DistritoInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionDireccionExactaError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                   EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.DireccionExactaInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveValidacionFechaVencimientoMenorHoyError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(-5),
                    EstadoEstablecimiento= "Activo"
            } };

            var error = ErroresEstablecimiento.FechaVencimientoMenorHoy;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }
        [Fact]
        public async Task ImportarDatos_DevuelveValidacionEstadoEstablecimientoError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(5),
                    EstadoEstablecimiento= "ActivoQWERTYUIOPLKJHGF"
            } };

            var error = ErroresEstablecimiento.EstadoEstablecimientoInvalido;

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ValidarEstablecimiento(datos[0].Id, datos[0].NumeroCvo)).ReturnsAsync(true);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert                        
            Assert.True(result.IsError);
            Assert.Equal(error, result.FirstError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarEstablecimientoDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                    NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(-5),
                    EstadoEstablecimiento= "Activo"
            } };
            var Establecimiento = new List<Dominio.Entidades.Establecimiento>()
            {
                new Dominio.Entidades.Establecimiento
                {
                     NumeroCvo = "123456",
                    NombreEstablecimiento= "Nombre Establecimiento 1",
                    Distrito = "Distrito 1",
                    DireccionExacta = "Direccion",
                    ActividadPrimaria = "Actividad 1",
                    ActividadSecundaria = "Actividad 2",
                    Provincia = "Provincia 1",
                    Canton = "Canton 1",
                    FechaVencimiento = DateTime.Now.AddDays(-5),
                  EstadoEstablecimiento= "Activo"
                }
            };
           
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(Establecimiento);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(It.IsAny<int>())).ReturnsAsync(Error.Failure());
          
            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_CrearEstablecimientoDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>()
            {
                new Dominio.Entidades.Establecimiento()
                {
                NumeroCvo = "123456",
                NombreEstablecimiento = "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(5),
              EstadoEstablecimiento= "Activo"
                }
            };

            var establecimiento = new List<Dominio.Entidades.Establecimiento>();

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(establecimiento);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.CrearEstablecimiento(It.IsAny<Dominio.Entidades.Establecimiento>())).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_DevuelveOK()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento()
            {
                NumeroCvo = "123456",
                NombreEstablecimiento = "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(5),
               EstadoEstablecimiento= "Activo"
                }
            };
            
            
            var establecimiento = new List<Dominio.Entidades.Establecimiento>();

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(establecimiento);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.CrearEstablecimiento(It.IsAny<Dominio.Entidades.Establecimiento>())).ReturnsAsync(datos[0]);

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
       
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento() {
                NumeroCvo = "123456",
                NombreEstablecimiento= "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(-5),
               EstadoEstablecimiento= "Activo"
        },
        new Dominio.Entidades.Establecimiento() {
                NumeroCvo = "123456",
                NombreEstablecimiento= "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(-5),
                EstadoEstablecimiento= "Activo"
        }

    };


            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);

            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(It.IsAny<int>())).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.CrearEstablecimiento(It.IsAny<Dominio.Entidades.Establecimiento>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_EliminarEstablecimientosDevuelveError()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento()
            {
                Id =1,
                NumeroCvo = "123456",
                NombreEstablecimiento= "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "Actividad 2",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(5),
                EstadoEstablecimiento= "Activo"
                    }
            };
            
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(Error.Failure());

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task ImportarDatos_ActividadSecundariaEspacios()
        {
            // Arrange
            var modo = 2;
            var datos = new List<Dominio.Entidades.Establecimiento>() { new Dominio.Entidades.Establecimiento()
            {
                NumeroCvo = "123456",
                NombreEstablecimiento = "Nombre Establecimiento 1",
                Distrito = "Distrito 1",
                DireccionExacta = "Direccion",
                ActividadPrimaria = "Actividad 1",
                ActividadSecundaria = "   ",
                Provincia = "Provincia 1",
                Canton = "Canton 1",
                FechaVencimiento = DateTime.Now.AddDays(5),
               EstadoEstablecimiento= "Activo"
                }
            };


            var establecimiento = new List<Dominio.Entidades.Establecimiento>();

            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(establecimiento);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.ObtenerEstablecimientos()).ReturnsAsync(datos);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.EliminarEstablecimiento(1)).ReturnsAsync(Result.Deleted);
            mockRepo.Setup(repo => repo.EstablecimientosRepository.CrearEstablecimiento(It.IsAny<Dominio.Entidades.Establecimiento>())).ReturnsAsync(datos[0]);

            var handler = new ImportarDatosCommandHandler(mockRepo.Object);
            // Act
            var result = await handler.Handle(new ImportarDatosCommand { Modo = modo, Datos = datos }, default);

            // Assert
            Assert.False(result.IsError);
        }
    }
}


 
