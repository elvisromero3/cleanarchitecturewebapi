using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CatalogosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public CatalogosRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        }

        [Fact]
        public async Task ObtenerCatalogoPorId_DevuelveOk()
        {
            int idCatalogo = 1;

            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Catalogos.Add(new Dominio.Entidades.Catalogo
                {
                    Id = idCatalogo,
                    Alias = "Alias 1",
                    Nombre = "Catálogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 1",
                    NombreIngles = "aaa",
                    NombreNormalizado = "Catalogo 1",
                    NombreInglesNormalizado = "aaa"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var catalogo = await repository.ObtenerCatalogoPorId(idCatalogo);

                Assert.False(catalogo.IsError);
                Assert.Equal(1, catalogo.Value.Id);
            }
        }

        [Fact]
        public async Task ObtenerCatalogoPorId_DevuelveError()
        {
            int idCatalogo = 1;

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var catalogo = await repository.ObtenerCatalogoPorId(idCatalogo);

                Assert.True(catalogo.IsError);
            }
        }

        [Fact]
        public async Task ObtenerCatalogos_DevuelveOk()
        {
            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Catalogos.Add(new Dominio.Entidades.Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catálogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 1",
                    NombreIngles = "aaa",
                    NombreNormalizado = "Catalogo 1",
                    NombreInglesNormalizado = "aaa"
                });

                context.Catalogos.Add(new Dominio.Entidades.Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catálogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 2",
                    NombreIngles = "aaa",
                    NombreNormalizado = "Catalogo 2",
                    NombreInglesNormalizado = "aaa"
                });

                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var catalogos = await repository.ObtenerCatalogos();

                Assert.False(catalogos.IsError);
                Assert.NotNull(catalogos.Value);
                Assert.IsType<List<Catalogo>>(catalogos.Value);
            }
        }

        [Fact]
        public async Task CrearCatalogo_DevuelveOk()
        {
            var catalogo = new Catalogo
            {
                Alias = "Alias 1",
                Nombre = "Catalogo 1",
                VisibleEmpresa = true,
                RequiereFirma = false,
                AccionControlador = "Coleccion 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var result = await repository.CrearCatalogo(catalogo);

                //Assert
                Assert.False(result.IsError);
                Assert.IsType<Catalogo>(result.Value);
            }
        }

        [Fact]
        public async Task ActualizarCatalogo_DevuelveOk()
        {
            var catalogo = new Catalogo
            {
                Id = 1,
                Alias = "Alias 1",
                Nombre = "Catálogo 1",
                VisibleEmpresa = true,
                RequiereFirma = false,
                AccionControlador = "Coleccion 1",
                NombreIngles = "aaa",
                NombreNormalizado = "Catalogo 1",
                NombreInglesNormalizado = "aaa"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Catalogos.Add(catalogo);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var result = await repository.ActualizarCatalogo(catalogo);

                //Assert
                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task EliminarCatalogo_DevuelveOk()
        {
            var catalogo = new Catalogo
            {
                Id = 1,
                Alias = "Alias 1",
                Nombre = "Catálogo 1",
                VisibleEmpresa = true,
                RequiereFirma = false,
                AccionControlador = "Coleccion 1",
                NombreIngles = "aaa",
                NombreNormalizado = "Catalogo 1",
                NombreInglesNormalizado = "aaa"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Catalogos.Add(catalogo);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var result = await repository.EliminarCatalogo(catalogo.Id);

                //Assert
                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task EliminarCatalogo_DevuelveError()
        {
            var catalogo = new Catalogo
            {
                Id = 1,
                Alias = "Alias 1",
                Nombre = "Catalogo 1",
                VisibleEmpresa = true,
                RequiereFirma = false,
                AccionControlador = "Coleccion 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var result = await repository.EliminarCatalogo(-1);

                //Assert
                Assert.True(result.IsError);
            }
        }

        [Fact]
        public async Task ObtenerCatalogosPorIdInstitucion_DevuelveOk()
        {
            int idInstitucion = 1;

            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Catalogos.Add(new Dominio.Entidades.Catalogo
                {
                    Id = 1,
                    Alias = "Alias 1",
                    Nombre = "Catálogo 1",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 1",
                    NombreIngles = "aaa",
                    NombreNormalizado = "Catalogo 1",
                    NombreInglesNormalizado = "aaa"
                });

                context.Catalogos.Add(new Dominio.Entidades.Catalogo
                {
                    Id = 2,
                    Alias = "Alias 2",
                    Nombre = "Catalogo 2",
                    VisibleEmpresa = true,
                    RequiereFirma = false,
                    AccionControlador = "Coleccion 2",
                    NombreIngles = "aaa",
                    NombreNormalizado = "Catálogo 2",
                    NombreInglesNormalizado = "aaa"
                });

                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CatalogosRepository(context);
                var catalogos = await repository.ObtenerCatalogosPorIdInstitucion(idInstitucion);

                Assert.False(catalogos.IsError);
                Assert.NotNull(catalogos.Value);
                Assert.IsType<List<Catalogo>>(catalogos.Value);
            }
        }

    }
}
