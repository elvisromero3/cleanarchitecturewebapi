using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresPaises
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Pais.NoEncontrado",
            description: "País no encontrado");

        public readonly static Error DatosDuplicados = Error.Validation(
           code: "Pais.DatosDuplicados",
           description: "Ya existe un país con los datos proporcionados");

        public readonly static Error NombreDuplicado = Error.Validation(
           code: "Pais.NombreDuplicado",
           description: "Ya existe un país con uno de los datos proporcionados (Nombre)");

        public readonly static Error CodigoA2Duplicado = Error.Validation(
           code: "Pais.CodigoA2Duplicado",
           description: "Ya existe un país con uno de los datos proporcionados (Código A2)");

        public readonly static Error CodigoC3Duplicado = Error.Validation(
           code: "Pais.CodigoC3Duplicado",
           description: "Ya existe un país con uno de los datos proporcionados (Código C3)");

        public readonly static Error CodigoNumericoDuplicado = Error.Validation(
           code: "Pais.CodigoNumericoDuplicado",
           description: "Ya existe un país con uno de los datos proporcionados (Código numérico)");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
           code: "Pais.DatosDuplicadosArchivo",
           description: "Ya existe un país con los datos proporcionados");

        public readonly static Error ValidacionNombre = Error.Validation(
          code: "Pais.ValidacionNombre",
          description: "El nombre debe tener máximo 100 caracteres");
        
        public readonly static Error CodigoA2ExcedeLimite = Error.Validation(
          code: "ValidacionCodigoA2",
          description: "El código A2 debe tener exactamente 2 caracteres mayúsculas de la A a la Z");

        public readonly static Error CodigoNumericoExcedeLimite = Error.Validation(
          code: "ValidacionCodigoNumerico",
          description: "El código numérico debe tener exactamente 3 números");

        public readonly static Error CodigoC3ExcedeLimite = Error.Validation(
          code: "ValidacionCodigoC3",
          description: "El código C3 debe tener exactamente 3 caracteres mayúsculas de la A a la Z");
        public readonly static Error PerteneceBloqueComercial = Error.Validation(
          code: "Pais.PerteneceBloqueComercial",
          description: "Pais pertenece a bloque comercial");
        public readonly static Error PerteneceRequisito = Error.Validation(
         code: "Pais.PerteneceRequisito",
         description: "Pais pertenece a Requisito");

    }
}
