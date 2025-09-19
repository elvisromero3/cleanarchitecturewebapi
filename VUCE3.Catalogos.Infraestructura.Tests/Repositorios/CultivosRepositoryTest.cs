using Microsoft.EntityFrameworkCore;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;

namespace VUCE3.Catalogos.Infraestructura.Tests.Repositorios
{
    public class CultivosRepositoryTest
    {
        private DbContextOptions<CatalogosDbContext> dbContextOptions;

        public CultivosRepositoryTest()
        {
            var dbName = $"BDTestCatalogos_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<CatalogosDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        }

        [Fact]
        public async Task ObtenerCultivos()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.AddRange(
                new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
                },
                new Cultivo
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    Variedad = new Variedad { Id = 1, Codigo = "1", Nombre = "Variedad" }
                },
                new Cultivo
                {
                    Id = 3,
                   Codigo = "3",
                    Nombre = "Cultivo 3",
                    NombreCientifico = "Cientifico 3",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ObtenerCultivos();

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<List<Cultivo>>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerCultivoPorId()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.AddRange(
                  new Cultivo
                  {
                      Id = 1,
                      Codigo = "1",
                      Nombre = "Cultivo 1",
                      NombreCientifico = "Cientifico 1",
                      Variedad = new Variedad { Id = 1, Codigo = "1", Nombre = "Variedad" }
                  },
                new Cultivo
                {
                    Id = 2,
                   Codigo = "2",
                    Nombre = "Cultivo 2",
                    NombreCientifico = "Cientifico 2",
                    Variedad = new Variedad { Id = 2,Codigo = "2", Nombre = "Variedad 2" }
                },
                new Cultivo
                {
                    Id = 3,
                   Codigo = "3",
                    Nombre = "Cultivo 3",
                    NombreCientifico = "Cientifico 3",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ObtenerCultivoPorId(2);

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<Cultivo>(result.Value);
            }
        }

        [Fact]
        public async Task ObtenerCultivoPorId_NoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.AddRange(
                new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ObtenerCultivoPorId(4);

                //Assert
                Assert.True(result.IsError);
                Assert.Null(result.Value);
            }
        }

        [Fact]
        public async Task CrearCultivo()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Variedades.Add(new Variedad { Id = 1, Codigo = "10", Nombre = "Variedad" });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.CrearCultivo(new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    IdVariedad = 1,
                    Variedad = new Variedad { Id = 1 }
                });

                Assert.False(result.IsError);
                Assert.NotNull(result.Value);
                Assert.IsType<Cultivo>(result.Value);
            }
        }

        [Fact]
        public async Task EditarCultivo()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ObtenerCultivoPorId(1);

                result.Value.Codigo = "1";
                result.Value.Nombre = "Cultivo 1 Editado";
                result.Value.NombreCientifico = "Cientifico 1 Editado";

                var listaCambios = new List<string> { "Nombre", "NombreCientifico" };

                var resultEdit = await repository.EditarCultivo(result.Value, result.Value.Id, listaCambios);

                Assert.False(resultEdit.IsError);
            }
        }

        [Fact]
        public async Task EditarCultivo_NoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ObtenerCultivoPorId(1);

                result.Value.Codigo = "1";
                result.Value.Nombre = "Cultivo 1 Editado";
                result.Value.NombreCientifico = "Cientifico 1 Editado";

                var listaCambios = new List<string> { "Nombre", "NombreCientifico" };

                var resultEdit = await repository.EditarCultivo(result.Value, 2, listaCambios);

                Assert.True(resultEdit.IsError);
            }
        }

        [Fact]
        public async Task EliminarCultivo()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.EliminarCultivo(1);

                Assert.False(result.IsError);
            }
        }

        [Fact]
        public async Task EliminarCultivo_NoEncontrado()
        {
            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(new Cultivo
                {
                    Id = 1,
                    Codigo = "1",
                    Nombre = "Cultivo 1",
                    NombreCientifico = "Cientifico 1",
                    Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
                });
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.EliminarCultivo(2);

                Assert.True(result.IsError);
            }
        }

        [Fact]
        public async Task CultivoDuplicada_DevuelveTrue()
        {

            // Arrange
            var cultivo = new Cultivo()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 3,Codigo = "3", Nombre = "Variedad 3" }
            };

            var cultivo2 = new Cultivo()
            {
                Id = 2,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 3, Codigo = "3", Nombre = "Variedad 3" }
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            
                var repository = new CultivosRepository(context);
                var result = await repository.ValidarCultivo(cultivo2.Id,cultivo2.Codigo,cultivo2.Nombre,cultivo2.NombreCientifico);

                Assert.True(result.Value);
            }
        }


        [Fact]
        public async Task CultivoDuplicada_DevuelveFalse()
        {

            // Arrange
            var cultivo = new Cultivo()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 3, Codigo = "3", Nombre = "Variedad 3" }
            };

            var cultivo2 = new Cultivo()
            {
                Id = 1,
                Codigo = "2",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 1, Codigo = "1", Nombre = "Variedad 1" }
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ValidarCultivo(cultivo2.Id,cultivo2.Codigo,cultivo2.Nombre,cultivo2.NombreCientifico);

                Assert.False(result.Value);
            }
        }

        [Fact]
        public async Task CultivoDuplicada_MismoDevuelveFalse()
        {

            // Arrange
            var cultivo = new Cultivo()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 3, Codigo = "3", Nombre = "Variedad 3" }
            };

            var cultivo2 = new Cultivo()
            {
                Id = 1,
                Codigo = "1",
                Nombre = "Cultivo 1",
                NombreCientifico = "Cientifico 1",
                Variedad = new Variedad { Id = 3, Codigo = "3", Nombre = "Variedad 3" }
            };

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                context.Cultivos.Add(cultivo);
                context.SaveChanges();
            }

            using (var context = new CatalogosDbContext(dbContextOptions))
            {
                var repository = new CultivosRepository(context);
                var result = await repository.ValidarCultivo(cultivo2.Id, cultivo2.Codigo, cultivo2.Nombre, cultivo2.NombreCientifico);

                Assert.False(result.Value);
            }
        }
    }
}
