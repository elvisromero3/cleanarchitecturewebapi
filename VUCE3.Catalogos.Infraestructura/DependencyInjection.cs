using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Infraestructura.Persistencia;
using VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios;
using VUCE3.Catalogos.Infraestructura.Servicios;
using Polly.Extensions.Http;
using Polly;
namespace VUCE3.Catalogos.Infraestructura
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddPersistence(configuration);

            services.AddAuth(configuration);

            services.AddServices(configuration);

            return services;
        }

        private static IServiceCollection AddAuth(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<CatalogosDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("database"))
            );
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<IVariedadesRepository, VariedadesRepository>();
            services.AddScoped<ICultivosRepository, CultivosRepository>();
            services.AddScoped<IPaisesRepository, PaisesRepository>();
            services.AddScoped<IEmpresasRepository, EmpresasRepository>();
            services.AddScoped<IProfesionalesRepository, ProfesionalesRepository>();
            services.AddScoped<IAduanasRepository, AduanasRepository>();
            services.AddScoped<IClientesRepository, ClientesRepository>();
            services.AddScoped<IExcepcionesMorosidadRepository, ExcepcionesMorosidadRepository>();       
            services.AddScoped<ITarifasRepository, TarifasRepository>();            
            services.AddScoped<IMonedasRepository, MonedasRepository>();
            services.AddScoped<IFamiliaRepository, FamiliaRepository>();
            services.AddScoped<IPaisBloqueComercialRepository, PaisBloqueComercialRepository>();


            services.AddAzureClients(clientBuilder =>
            {
                // Register clients for each service
                clientBuilder.AddBlobServiceClient(configuration["Blob:ConnectionString"]);
            });

            services.AddHttpClient<IAccesoGestionUsuariosService, AccesoGestionUsuariosService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(configuration["Apis:AccesoGestionUsuarios:BaseUrl"] ?? "");
            }).AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<ITramitesService, TramitesService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(configuration["Apis:Tramites:BaseUrl"] ?? "");
            }).AddPolicyHandler(GetRetryPolicy());

            return services;
        }
        private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5));
        }
    }
}
