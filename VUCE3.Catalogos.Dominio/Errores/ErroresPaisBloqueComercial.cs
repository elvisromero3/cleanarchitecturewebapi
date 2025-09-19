using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresPaisBloqueComercial
    {

        public readonly static Error NoEncontrada = Error.NotFound(
            code: "PaisBloqueComercial.NoEncontrada",
            description: "Pais de bloque comercial no encontrada");

        public readonly static Error DatosDuplicados = Error.Validation(
           code: "PaisBloqueComercial.DatosDuplicados",
           description: "Ya existe un país de bloque comercial con los datos proporcionados");

        public readonly static Error PaisNoEncontrado = Error.NotFound(
         code: "PaisBloqueComercial.PaisNoEncontrado",
         description: "Pais no encontrado");

        public readonly static Error BloqueComercialNoEncontrado = Error.NotFound(
         code: "PaisBloqueComercial.BloqueComercialNoEncontrado",
         description: "Bloque comercial no encontrado");

        public readonly static Error PaisBloqueComercialDatosDuplicadosArchivo = Error.Validation(
           code: "PaisBloqueComercial.DatosDuplicadosArchivo",
           description: "El archivo contiene un pais repetido");
    }
}
