using System.Diagnostics.CodeAnalysis;
using VUCE3.Catalogos.Dominio.Constantes;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace VUCE3.Catalogos.Aplicacion
{
    [ExcludeFromCodeCoverage]
    public static partial class Validadores
    {
        public static bool EmailInvalido(string email)
        {
            Regex regexEmail = new Regex(@"^[\w\.-]+@([\w-]+\.)+[\w-]+$", RegexOptions.None, TimeSpan.FromSeconds(3));

            return !regexEmail.IsMatch(email);
        }
        
        public static bool UrlInvalida(string url)
        {
            Regex regexUrl = new Regex(@"^(https?:\/\/)?([\w\-]+(\.[\w\-]+)+)(\/[\w\-._~:\/?#\[\]&'()*+,;=]*)?$", RegexOptions.None, TimeSpan.FromSeconds(3));

            return !regexUrl.IsMatch(url); 
        }

        public static bool NumeroIdentificacion(char idTipoIdentificacion, string numeroIdentificacion)
        {
            Regex regexFisica = new Regex(@"^[0-9]{9}$", RegexOptions.None, TimeSpan.FromSeconds(3));
            if (idTipoIdentificacion == ConstantesIdTipoIdentificacion.FISICA && !regexFisica.IsMatch(numeroIdentificacion))
            {
                return true;
            }
            Regex regexJuridica = new Regex(@"^[0-9]{10}$", RegexOptions.None, TimeSpan.FromSeconds(3));
            if (idTipoIdentificacion == ConstantesIdTipoIdentificacion.JURIDICA && !regexJuridica.IsMatch(numeroIdentificacion))
            {
                return true;
            }
            Regex regexDimex = new Regex(@"^[a-zA-Z0-9]{12}$", RegexOptions.None, TimeSpan.FromSeconds(3));
            if (idTipoIdentificacion == ConstantesIdTipoIdentificacion.DIMEX && !regexDimex.IsMatch(numeroIdentificacion))
            {
                return true;
            }
            Regex regexPasaporte = new Regex(@"^[a-zA-Z0-9]{12}$", RegexOptions.None, TimeSpan.FromSeconds(3));
            if (idTipoIdentificacion == ConstantesIdTipoIdentificacion.PASAPORTE && !regexPasaporte.IsMatch(numeroIdentificacion))
            {
                return true; 
            }

            return false;
        }

        public static bool TipoIdentificacionFisicaComienza0(int idTipoIdentificacion, string numeroIdentificacion)
        {
            return idTipoIdentificacion == ConstantesIdTipoIdentificacion.FISICA && numeroIdentificacion.StartsWith('0');
        }

        [GeneratedRegex("^[A-Z]{2}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
        private static partial Regex ValidarCodigoA2Regex();
        public static bool IsValidoCodigoA2(string value)
        {
            return ValidarCodigoA2Regex().IsMatch(value);
        }

        [GeneratedRegex("^[0-9]{3}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
        private static partial Regex ValidarCodigoNumericoRegex();
        public static bool IsValidoCodigoNumerico(string value)
        {
            return ValidarCodigoNumericoRegex().IsMatch(value);
        }

        [GeneratedRegex("^[A-Z]{3}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
        private static partial Regex ValidarCodigoC3Regex();
        public static bool IsValidoCodigoC3(string value)
        {
            return ValidarCodigoC3Regex().IsMatch(value);
        }

        public static bool Nombre(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100;
        }

        public static bool Nombre50(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre) || nombre.Length > 50;
        }
        public static bool Nombre100(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100;
        }

        public static bool Codigo1(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 1;
        }

        public static bool Codigo3(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 3;
        }

        public static bool Codigo5(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 5;
        }

        public static bool Codigo7(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 7;
        }

        public static bool Codigo17(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 17;
        }

        public static bool Codigo35(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 35;
        }

        public static bool Codigo25(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 25;
        }

        public static bool Codigo20(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 20;
        }

        public static bool Codigo50(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo) || codigo.Length > 50;
        }

        public static bool Titulo(string titulo)
        {
            return string.IsNullOrWhiteSpace(titulo) || titulo.Length > 100;
        }

        public static bool Enlace(string texto)
        {
            return texto.Length > 250;
        }

        public static string NormalizarString(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var textoSinAcentos = texto.Normalize(NormalizationForm.FormD);
            char[] caracteres = textoSinAcentos.ToCharArray();
            var resultado = new StringBuilder();

            foreach (char c in caracteres)
            {
                UnicodeCategory categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark && categoria != UnicodeCategory.SpaceSeparator)
                {
                    resultado.Append(c);
                }
            }
            return resultado.ToString().ToLowerInvariant();
        }

        public static bool LongitudMaximaNoNull(string cadena, int longitud)
        {
            return string.IsNullOrWhiteSpace(cadena) || cadena.Length > longitud;
        }

        public static bool LongitudMaxima(string cadena, int longitud)
        {
            return cadena.Length > longitud;
        }

        public static bool institucionDCAoDIPOA(int istitucion)
        {
            return istitucion != ConstantesInstituciones.DCA && istitucion != ConstantesInstituciones.DIPOA;
        }
    }
}
