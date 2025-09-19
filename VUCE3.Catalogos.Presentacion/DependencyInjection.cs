using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Presentacion.DTO;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using VUCE3.Catalogos.Presentacion.Language;
using VUCE3.Catalogos.Aplicacion.Requisitos.Command.ImportarDatos.DTO;

namespace VUCE3.Catalogos.Presentacion
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(
            this IServiceCollection services,
            ConfigurationManager configuration,
            string nombrePoliticaCors)
        {           

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                         new CultureInfo("es"),
                         new CultureInfo("en")
                    };

                    options.DefaultRequestCulture = new RequestCulture("es");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            services.AddControllers()
                .AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata", GetEdmModel()));

            services.AddEndpointsApiExplorer();
            services.AddAutoMapper(typeof(Program));

            var AllowHostCors = configuration["AllowHostCors"]?.Split(",");
            services.AddCors(o =>
                o.AddPolicy(nombrePoliticaCors,
                    builder =>
                    {
                        builder.WithOrigins(AllowHostCors ?? [])
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader();

                    }
                )

            );

            services.AddSwaggerGen(c =>
            {
                var bearerString = "Bearer";

                c.AddSecurityDefinition(bearerString, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = bearerString
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = bearerString
                            },
                            Scheme = "oauth2",
                            Name = bearerString,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });                
                
                c.OperationFilter<LanguageHeaderParameter>();

            });

            

            return services;
        }

        private static IEdmModel GetEdmModel()
        {
            var nombreBorradoMasivo = "BorradoMasivo";
            var nombreItems = "items";
            var nombreImportarDatos = "ImportarDatos";
            var nombreDatos = "datos";
            var nombreModo = "modo";

            var builder = new ODataConventionModelBuilder();

            //pais
            var pais = builder.EntitySet<PaisDto>("Paises").EntityType.HasKey(x => x.Id);
            var borradoMasivoPaises = pais.Collection.Action(nombreBorradoMasivo);
            borradoMasivoPaises.CollectionParameter<int>(nombreItems);

            var importarPaises = pais.Collection.Action(nombreImportarDatos);
            importarPaises.CollectionParameter<ImportarPaisDto>(nombreDatos);
            importarPaises.Parameter<int>(nombreModo);

            //profesional
            var profesional = builder.EntitySet<ProfesionalDto>("Regentes").EntityType.HasKey(x => x.Id);
            var borradoMasivoProfesionales = profesional.Collection.Action(nombreBorradoMasivo);
            borradoMasivoProfesionales.CollectionParameter<int>(nombreItems);

            var importarProfesionales = profesional.Collection.Action(nombreImportarDatos);
            importarProfesionales.CollectionParameter<ImportarProfesionalDto>(nombreDatos);
            importarProfesionales.Parameter<int>(nombreModo);

            //empresa
            var empresa = builder.EntitySet<EmpresaDto>("Empresas").EntityType.HasKey(x => x.Id);
            var borradoMasivoEmpresa = empresa.Collection.Action(nombreBorradoMasivo);
            borradoMasivoEmpresa.CollectionParameter<int>(nombreItems);

            var importarDatosEmpresa = empresa.Collection.Action(nombreImportarDatos);
            importarDatosEmpresa.CollectionParameter<ImportarEmpresaDto>(nombreDatos);
            importarDatosEmpresa.Parameter<int>(nombreModo);

            //variedad
            var variedad = builder.EntitySet<VariedadDto>("Variedad").EntityType.HasKey(x => x.Id);
            var importarDatosVariedad = variedad.Collection.Action(nombreImportarDatos);
            importarDatosVariedad.CollectionParameter<ImportarVariedadDto>(nombreDatos);
            importarDatosVariedad.Parameter<int>(nombreModo);

            var borradoMasivoVariedades = variedad.Collection.Action(nombreBorradoMasivo);
            borradoMasivoVariedades.CollectionParameter<int>(nombreItems);

            //casa
            var casa = builder.EntitySet<CasaDto>("Casa").EntityType.HasKey(x => x.Id);
            var borradoMasivoCasas = casa.Collection.Action(nombreBorradoMasivo);
            borradoMasivoCasas.CollectionParameter<int>(nombreItems);

            var importarDatosCasa = casa.Collection.Action(nombreImportarDatos);
            importarDatosCasa.CollectionParameter<ImportarCasaDto>(nombreDatos);
            importarDatosCasa.Parameter<int>(nombreModo);

            //cultivo
            var cultivo = builder.EntitySet<CultivoDto>("Cultivo").EntityType.HasKey(x => x.Id);
            var borradoMasivoCultivo = cultivo.Collection.Action(nombreBorradoMasivo);
            borradoMasivoCultivo.CollectionParameter<int>(nombreItems);

            var importarDatosCultivo = cultivo.Collection.Action(nombreImportarDatos);
            importarDatosCultivo.CollectionParameter<ImportarCultivoDto>(nombreDatos);
            importarDatosCultivo.Parameter<int>(nombreModo);

            //tarifa
            builder.EntitySet<TarifaDto>("Tarifas").EntityType.HasKey(x => x.Id);

            //moneda
            builder.EntitySet<MonedaDto>("Monedas").EntityType.HasKey(x => x.Id);

            //aduana
            var aduana = builder.EntitySet<AduanaDto>("Aduanas").EntityType.HasKey(x => x.Id);
            var borradoMasivoAduanas = aduana.Collection.Action(nombreBorradoMasivo);
            borradoMasivoAduanas.CollectionParameter<int>(nombreItems);

            var importarDatosAduana = aduana.Collection.Action(nombreImportarDatos);
            importarDatosAduana.CollectionParameter<ImportarAduanaDto>(nombreDatos);
            importarDatosAduana.Parameter<int>(nombreModo);

            //catalogo
            var catalogo = builder.EntitySet<CatalogoDto>("Catalogos").EntityType.HasKey(x => x.Id);

            var catalogosPorInstitucion = catalogo.Collection.Action("ObtenerCatalogosPorInstitucion");
            catalogosPorInstitucion.Parameter<int>("idInstitucion");

            //cliente
            var cliente = builder.EntitySet<ClienteDto>("Clientes").EntityType.HasKey(x => x.Id);
            var borradoMasivoClientes = cliente.Collection.Action(nombreBorradoMasivo);
            borradoMasivoClientes.CollectionParameter<int>(nombreItems);

            var importarDatosCliente = cliente.Collection.Action(nombreImportarDatos);
            importarDatosCliente.CollectionParameter<ImportarClienteDto>(nombreDatos);
            importarDatosCliente.Parameter<int>(nombreModo);

            //noticias vuce 
            var noticiasVuce = builder.EntitySet<NoticiasVuceDto>("NoticiasVuce").EntityType.HasKey(x => x.Id);
            var importarDatosNoticiasVuce = noticiasVuce.Collection.Action(nombreImportarDatos);
            importarDatosNoticiasVuce.CollectionParameter<ImportarNoticiasVuceDto>(nombreDatos);
            importarDatosNoticiasVuce.Parameter<int>(nombreModo);

            var borradoMasivoNoticiasVuce = noticiasVuce.Collection.Action(nombreBorradoMasivo);
            borradoMasivoNoticiasVuce.CollectionParameter<int>(nombreItems);

            //imagenes
            var imagen = builder.EntitySet<ImagenesDto>("Imagenes").EntityType.HasKey(x => x.Id);
            var borradoMasivoImagenes = imagen.Collection.Action(nombreBorradoMasivo);
            borradoMasivoImagenes.CollectionParameter<int>(nombreItems);

            //ExcepcionMorosidad
            var excepcionMorosidad = builder.EntitySet<ExcepcionMorosidadDto>("ExcepcionMorosidad").EntityType.HasKey(x => x.Id);

            var importarDatosExcepcionMorosidad = excepcionMorosidad.Collection.Action(nombreImportarDatos);
            importarDatosExcepcionMorosidad.CollectionParameter<ImportarExcepcionMorosidadDto>(nombreDatos);
            importarDatosExcepcionMorosidad.Parameter<int>(nombreModo);

            var borradoMasivoexcepcioneMorosidad = excepcionMorosidad.Collection.Action(nombreBorradoMasivo);
            borradoMasivoexcepcioneMorosidad.CollectionParameter<int>(nombreItems);

            //Estado Excepción Morosidad
            builder.EntitySet<EstadoExcepcionMorosidadDto>("EstadosExcepcionesMorosidad").EntityType.HasKey(x => x.Id);
            //Tipo Acción Excepción Morosidad
            builder.EntitySet<TipoAccionExcepcionMorosidadDto>("TiposAccionesExcepcionesMorosidad").EntityType.HasKey(x => x.Id);
            

            //Sector
            var sector = builder.EntitySet<SectorDto>("Sectores").EntityType.HasKey(x => x.Id);

            var borradoMasivoSectores = sector.Collection.Action(nombreBorradoMasivo);
            borradoMasivoSectores.CollectionParameter<int>(nombreItems);

            var importarDatosSector = sector.Collection.Action(nombreImportarDatos);
            importarDatosSector.CollectionParameter<ImportarSectorDto>(nombreDatos);
            importarDatosSector.Parameter<int>(nombreModo);

            //familia
            var familia = builder.EntitySet<FamiliaDto>("Familia").EntityType.HasKey(x => x.Id);
            var borradoMasivoFamilias = familia.Collection.Action(nombreBorradoMasivo);
            borradoMasivoFamilias.CollectionParameter<int>(nombreItems);

            var importarDatosFamilia = familia.Collection.Action(nombreImportarDatos);
            importarDatosFamilia.CollectionParameter<ImportarFamiliaDto>(nombreDatos);
            importarDatosFamilia.Parameter<int>(nombreModo);

            //Caracteristica
            var caracteristica = builder.EntitySet<CaracteristicaDto>("Caracteristicas").EntityType.HasKey(x => x.Id);

            var importarDatosCaracteristicas = caracteristica.Collection.Action(nombreImportarDatos);
            importarDatosCaracteristicas.CollectionParameter<ImportarCaracteristicaDto>(nombreDatos);
            importarDatosCaracteristicas.Parameter<int>(nombreModo);

            var borradoMasicoCaracteristicas = caracteristica.Collection.Action(nombreBorradoMasivo);
            borradoMasicoCaracteristicas.CollectionParameter<int>(nombreItems);

            //Bloque comercial
            var bloqueComercial = builder.EntitySet<BloqueComercialDto>("BloquesComerciales").EntityType.HasKey(x => x.Id);
            
            var importarDatosBloqueComercial = bloqueComercial.Collection.Action(nombreImportarDatos);
            importarDatosBloqueComercial.CollectionParameter<ImportarBloqueComercialDto>(nombreDatos);
            importarDatosBloqueComercial.Parameter<int>(nombreModo);

            var borradoMasivoBloqueComercial = bloqueComercial.Collection.Action(nombreBorradoMasivo);
            borradoMasivoBloqueComercial.CollectionParameter<int>(nombreItems);

            //Categoria
            var categoria = builder.EntitySet<CategoriaDto>("Categoria").EntityType.HasKey(x => x.Id);

            var importarDatosCategoria = categoria.Collection.Action(nombreImportarDatos);
            importarDatosCategoria.CollectionParameter<ImportarCategoriaDto>(nombreDatos);
            importarDatosCategoria.Parameter<int>(nombreModo);
            var borradoMasivoCategoria = categoria.Collection.Action(nombreBorradoMasivo);
            borradoMasivoCategoria.CollectionParameter<int>(nombreItems);

            //TipoProductos
            var tipoProductos = builder.EntitySet<TipoProductoDto>("TipoProductos").EntityType.HasKey(x => x.Id);
            var importarDatosTipoProductos = tipoProductos.Collection.Action(nombreImportarDatos);
            importarDatosTipoProductos.CollectionParameter<ImportarTipoProductoDto>(nombreDatos);
            importarDatosTipoProductos.Parameter<int>(nombreModo);
            var borradoMasivoTipoProductos = tipoProductos.Collection.Action(nombreBorradoMasivo);
            borradoMasivoTipoProductos.CollectionParameter<int>(nombreItems);

            //Provincia
            var provincia = builder.EntitySet<ProvinciaDto>("Provincias").EntityType.HasKey(x => x.Id);
            var borradoMasivoProvincias = provincia.Collection.Action(nombreBorradoMasivo);
            borradoMasivoProvincias.CollectionParameter<int>(nombreItems);

            var importarDatosProvincia = provincia.Collection.Action(nombreImportarDatos);
            importarDatosProvincia.CollectionParameter<ImportarProvinciaDto>(nombreDatos);
            importarDatosProvincia.Parameter<int>(nombreModo);

            //Cantones
            var canton = builder.EntitySet<CantonDto>("Cantones").EntityType.HasKey(x => x.Id);
            var borradoMasivoCantones = canton.Collection.Action(nombreBorradoMasivo);
            borradoMasivoCantones.CollectionParameter<int>(nombreItems);

            var importarDatosCantones = canton.Collection.Action(nombreImportarDatos);
            importarDatosCantones.CollectionParameter<ImportarCantonDto>(nombreDatos);
            importarDatosCantones.Parameter<int>(nombreModo);

            //Distritos
            var distrito = builder.EntitySet<DistritoDto>("Distritos").EntityType.HasKey(x => x.Id);
            var borradoMasivoDistritos = distrito.Collection.Action(nombreBorradoMasivo);
            borradoMasivoDistritos.CollectionParameter<int>(nombreItems);

            var importarDatosdistritos = distrito.Collection.Action(nombreImportarDatos);
            importarDatosdistritos.CollectionParameter<ImportarDistritoDto>(nombreDatos);
            importarDatosdistritos.Parameter<int>(nombreModo);

            //Barrios
            var barrio = builder.EntitySet<BarrioDto>("Barrios").EntityType.HasKey(x => x.Id);
            var borradoMasivoBarrios = barrio.Collection.Action(nombreBorradoMasivo);
            borradoMasivoBarrios.CollectionParameter<int>(nombreItems);

            var importarDatosBarrios = barrio.Collection.Action(nombreImportarDatos);
            importarDatosBarrios.CollectionParameter<ImportarBarrioDto>(nombreDatos);
            importarDatosBarrios.Parameter<int>(nombreModo);

            //Sustancias
            var sustancia = builder.EntitySet<SustanciaDto>("Sustancia").EntityType.HasKey(x => x.Id);
            var borradoMasivoSustancias = sustancia.Collection.Action(nombreBorradoMasivo);
            borradoMasivoSustancias.CollectionParameter<int>(nombreItems);

            var importarDatosSustancia = sustancia.Collection.Action(nombreImportarDatos);
            importarDatosSustancia.CollectionParameter<ImportarSustanciaDto>(nombreDatos);
            importarDatosSustancia.Parameter<int>(nombreModo);

            //Requisitos
            var requisito = builder.EntitySet<RequisitoDto>("Requisitos").EntityType.HasKey(x => x.Id);
            
            var importarDatosRequisito = requisito.Collection.Action(nombreImportarDatos);
            importarDatosRequisito.CollectionParameter<ImportarRequisitoDto>(nombreDatos);
            importarDatosRequisito.Parameter<int>(nombreModo);

            var borradoMasivoRequisito = requisito.Collection.Action(nombreBorradoMasivo);
            borradoMasivoRequisito.CollectionParameter<int>(nombreItems);

            //CaracteristicaTipoProducto
            var caracteristicaTipoProducto = builder.EntitySet<CaracteristicaTipoProductoDto>("CaracteristicaTipoProductos").EntityType.HasKey(x => x.Id);
            
            var importarDatosCaracteristicaTipoProducto = caracteristicaTipoProducto.Collection.Action(nombreImportarDatos);
            importarDatosCaracteristicaTipoProducto.CollectionParameter<ImportarCaracteristicaTipoProductoDto>(nombreDatos);
            importarDatosCaracteristicaTipoProducto.Parameter<int>(nombreModo);

            var borradoMasivocaracteristicaTipoProducto = caracteristicaTipoProducto.Collection.Action(nombreBorradoMasivo);
            borradoMasivocaracteristicaTipoProducto.CollectionParameter<int>(nombreItems);

            //ProductoRequisto
            builder.EntitySet<ProductoRequisitoDto>("ProductoRequisitos").EntityType.HasKey(x => x.Id);
            var productoRequisito = builder.EntitySet<ProductoRequisitoDto>("ProductoRequisitos").EntityType.HasKey(x => x.Id);
            var importarDatosProductoRequisito = productoRequisito.Collection.Action(nombreImportarDatos);
            importarDatosProductoRequisito.CollectionParameter<ImportarProductoRequisitoDto>(nombreDatos);
            importarDatosProductoRequisito.Parameter<int>(nombreModo);

            var borradoMasivoProductoRequisito = productoRequisito.Collection.Action(nombreBorradoMasivo);
            borradoMasivoProductoRequisito.CollectionParameter<int>(nombreItems);

            //Productos
            var productos = builder.EntitySet<ProductosDto>("Productos").EntityType.HasKey(x => x.Id);

            var importarDatosProductos = productos.Collection.Action(nombreImportarDatos);
            importarDatosProductos.CollectionParameter<ImportarProductoDto>(nombreDatos);
            importarDatosProductos.Parameter<int>(nombreModo);

            var borradoMasivoProductos = productos.Collection.Action(nombreBorradoMasivo);
            borradoMasivoProductos.CollectionParameter<int>(nombreItems);

            //Sustancias Controladas
            var sustanciasControladas = builder.EntitySet<SustanciaControladaDto>("SustanciasControladas").EntityType.HasKey(x => x.Id);
            
            var borradoMasivoSustanciasControladas = sustanciasControladas.Collection.Action(nombreBorradoMasivo);
            borradoMasivoSustanciasControladas.CollectionParameter<int>(nombreItems);

            var importarDatosSustanciaControladas = sustanciasControladas.Collection.Action(nombreImportarDatos);
            importarDatosSustanciaControladas.CollectionParameter<ImportarSustanciaControladaDto>(nombreDatos);
            importarDatosSustanciaControladas.Parameter<int>(nombreModo);
       
            //PaisBloqueComercial
            var paisBloqueComercial = builder.EntitySet<PaisBloqueComercialDto>("PaisBloqueComercial").EntityType.HasKey(x => x.Id);
            var borradoMasivoPaisBloqueComercial = paisBloqueComercial.Collection.Action(nombreBorradoMasivo);
            borradoMasivoPaisBloqueComercial.CollectionParameter<int>(nombreItems);
            var importarDatosPaisBloqueComercial = paisBloqueComercial.Collection.Action(nombreImportarDatos);
            importarDatosPaisBloqueComercial.CollectionParameter<ImportarPaisBloqueComercialDto>(nombreDatos);
            importarDatosPaisBloqueComercial.Parameter<int>(nombreModo);

            //Establecimientos
            var establecimiento =  builder.EntitySet<EstablecimientoDto>("Establecimientos").EntityType.HasKey(x => x.Id);
            var borradoMasivoEstablecimiento = establecimiento.Collection.Action(nombreBorradoMasivo);
            borradoMasivoEstablecimiento.CollectionParameter<int>(nombreItems);
            var importarDatosEstablecimiento = establecimiento.Collection.Action(nombreImportarDatos);
            importarDatosEstablecimiento.CollectionParameter<ImportarEstablecimientoDto>(nombreDatos);
            importarDatosEstablecimiento.Parameter<int>(nombreModo);


            return builder.GetEdmModel();
        }

    }
}
