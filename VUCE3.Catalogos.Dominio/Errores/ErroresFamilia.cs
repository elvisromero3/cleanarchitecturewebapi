using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresFamilia
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Familia.NoEncontrada",
            description: "Familia no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Familia.DatosDuplicados",
           description: "Ya existe una familia con los datos proporcionados");

        public readonly static Error NombreInvalido = Error.Validation(
          code: "Familia.NombreInvalido",
          description: "La familia es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
         code: "Familia.DatosDuplicadosArchivo",
         description: "Familia contiene datos duplicados");

        public readonly static Error SustanciasControladasRelacionadas = Error.Validation(
         code: "Familia.SustanciasControladasRelacionadas",
         description: "La(s) familias(s) tiene(n) sustancias controladas relacionadas");

    }
}
