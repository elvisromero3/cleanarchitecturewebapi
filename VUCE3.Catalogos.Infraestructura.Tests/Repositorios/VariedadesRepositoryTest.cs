using ErrorOr;
using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class VariedadesRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public VariedadesRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        }

        [Fact]
        public async Task ObtenerVariedades_DevuelveOk()
        {
            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(new Dominio.Entidades.Variedad
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Variedad 1"
                });
                context.Variedades.Add(new Dominio.Entidades.Variedad
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Variedad 2"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var variedades = await repository.ObtenerVariedades();

                Assert.False(variedades.IsError);
                Assert.NotNull(variedades.Value);
                Assert.IsType<List<Dominio.Entidades.Variedad>>(variedades.Value);
            }
        }

        [Fact]
        public async Task ObtenerVariedadPorId_DevuelveOk()
        {
            int idVariedad = 1;

            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(new Dominio.Entidades.Variedad
                {
                    Id = idVariedad,
                    Codigo = "1",
                    Nombre = "Variedad 1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var variedad = await repository.ObtenerVariedadPorId(idVariedad);

                Assert.False(variedad.IsError);
                Assert.NotNull(variedad.Value);
                Assert.Equal(idVariedad, variedad.Value.Id);
            }
        }

        [Fact]
        public async Task ObtenerVariedadPorId_NoExiste_DevuelveError()
        {
            int idVariedad = 1;

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var variedad = await repository.ObtenerVariedadPorId(idVariedad);

                Assert.True(variedad.IsError);
            }
        }

        [Fact]
        public async Task ObtenerVariedadPorCodigo_DevuelveOk()
        {
            string codigoVariedad = "1";

            // Arrange
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(new Dominio.Entidades.Variedad
                {
                    Id = 1,
                    Codigo = codigoVariedad,
                    Nombre = "Variedad 1"
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var variedad = await repository.ObtenerVariedadPorCodigo(codigoVariedad);

                Assert.False(variedad.IsError);
                Assert.NotNull(variedad.Value);
                Assert.Equal(codigoVariedad, variedad.Value.Codigo);
            }
        }

        [Fact]
        public async Task ObtenerVariedadPorCodigo_NoExiste_DevuelveError()
        {
            string codigoVariedad = "1";

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var variedad = await repository.ObtenerVariedadPorCodigo(codigoVariedad);

                Assert.True(variedad.IsError);
            }
        }

        [Fact]
        public async Task CrearVariedad_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.CrearVariedad(variedad);

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.Equal(variedad.Id, result.Value.Id);
            }
        }

        [Fact]
        public async Task EditarVariedad_DevuelveOk()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };
            var listaCambios = new List<string> { "Nombre" };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }

            variedad.Codigo = "1";
            variedad.Nombre = "Variedad 1 Test";

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.EditarVariedad(variedad, variedad.Id, listaCambios);

                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task EditarVariedad_NoEncontrada()
        {
            var listaCambios = new List<string> { "Venezuela" };
            var variedadActualizada = new Variedad { Id = 999, Nombre = "Variedad1", Codigo = "1" };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);

                //Act
                var error = await repository.EditarVariedad(variedadActualizada, 999, listaCambios);
                Assert.True(error.IsError);
                Assert.Equal("Variedad.NoEncontrada", error.Errors[0].Code);
                Assert.Equal("Variedad no encontrada", error.Errors[0].Description);
            }
        }

        [Fact]
        public async Task EliminarVariedad_DevuelveOk()
        {
            int idVariedad = 1;

            // Arrange
            var variedad = new Dominio.Entidades.Variedad
            {
                Id = idVariedad,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.EliminarVariedad(idVariedad);

                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task EliminarVariedad_NoExiste_DevuelveError()
        {
            int idVariedad = 1;

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.EliminarVariedad(idVariedad);

                Assert.True(result.IsError);
            }
        }        

        [Fact]
        public async Task EliminarVariedadPorEntidad_DevuelveOk()
        {
            int idVariedad = 1;

            // Arrange
            var variedad = new Dominio.Entidades.Variedad
            {
                Id = idVariedad,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.EliminarVariedad(variedad);

                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task VariedadDuplicada_DevuelveTrue()
        {

            // Arrange
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            var variedad2 = new Variedad
            {
                Id = 2,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }            

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.ValidarVariedad(variedad2.Id,variedad2.Codigo, variedad2.Nombre);

                Assert.True(result.Value);
            }
        }

        [Fact]
        public async Task VariedadDuplicada_DevuelveFalse()
        {

            // Arrange
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            var variedad2 = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad mod"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.ValidarVariedad(variedad2.Id, variedad2.Codigo, variedad2.Nombre);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task VariedadDuplicada_MismoDevuelveFalse()
        {

            // Arrange
            var variedad = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            var variedad2 = new Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(variedad);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new VariedadesRepository(context);
                var result = await repository.ValidarVariedad(variedad2.Id,variedad2.Codigo, variedad2.Nombre);

                Assert.False(result.Value);
            }
        }


        [Fact]
        public async Task EliminarVariedad_RelacionCultivo()
        {
            // Arrange
            var variedad = new Dominio.Entidades.Variedad
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Variedad 1"
            };

            var cultivo = new Cultivo
            {
                Id=1,
               Codigo = "3",
                Nombre ="Cultivo 1",
                IdVariedad= 1,
                NombreCientifico ="Cientifico Cultivo1"
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                //
                context.Variedades.Add(variedad);
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
              
                var repository = new VariedadesRepository(context);
                var result = await repository.ExisteRelacionConCultivo(1);


                Assert.False(result.IsError);
                Assert.True(result.Value);
            }
        }
    }
}
