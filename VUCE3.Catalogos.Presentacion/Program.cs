using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Infraestructura;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Presentacion.Middlewares;
using VUCE3.Catalogos.Presentacion.Language;

namespace VUCE3.Catalogos.Presentacion
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var nombrePoliticaCors = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);
            var AllowHostCors = builder.Configuration["AllowHostCors"]?.Split(",");

            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddPresentation(builder.Configuration, nombrePoliticaCors);

            

            var app = builder.Build();

            app.UseRequestLocalization();
            app.UseSwagger();
            app.UseSwaggerUI();

            // Usa el middleware para manejar excepciones globalmente
            app.UseExceptionHandler("/odata/Error");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseCors(nombrePoliticaCors);

            app.UseMiddleware<SanitizationMiddleware>();

            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<AuditoriaMiddleware>();

            await app.RunAsync();
        }
    }
}