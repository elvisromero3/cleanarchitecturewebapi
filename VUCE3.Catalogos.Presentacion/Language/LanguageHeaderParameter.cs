using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace VUCE3.Catalogos.Presentacion.Language{

    [ExcludeFromCodeCoverage]
    public class LanguageHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var parameter = new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Language preference for the response",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }                
            };

            parameter.Schema.Enum.Add(new OpenApiString("es"));
            parameter.Schema.Enum.Add(new OpenApiString("en"));
            operation.Parameters.Add(parameter);
        }        
    }
}
