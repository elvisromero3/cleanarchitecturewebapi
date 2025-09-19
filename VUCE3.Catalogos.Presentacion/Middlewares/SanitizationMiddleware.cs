using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Text;

namespace VUCE3.Catalogos.Presentacion.Middlewares
{
    [ExcludeFromCodeCoverage]
    public partial class SanitizationMiddleware
    {
        private readonly RequestDelegate _next;

        public SanitizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.ContentType != null && request.ContentType.Contains("application/json"))
            {
                request.EnableBuffering();

                // Leer el cuerpo de la solicitud
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var body = await reader.ReadToEndAsync();

                    var sanitizedBody = SanitizarEntrada(body);

                    var bytes = Encoding.UTF8.GetBytes(sanitizedBody);
                    request.Body = new MemoryStream(bytes);
                    request.Body.Seek(0, SeekOrigin.Begin);
                }
            }

            await _next(context);
        }

       
        [GeneratedRegex("<\\/?\\s*script\\s*[^>]*>")]
        private static partial Regex HtmlTagRegex();

        private static string SanitizarEntrada(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var sanitized = HtmlTagRegex().Replace(input, string.Empty);
            sanitized = sanitized.Replace("'", string.Empty);
            return sanitized;
        }
    }
}
