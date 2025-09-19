using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresBloqueComercial
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "BloqueComercial.NoEncontrada",
            description: "BloqueComercial no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "BloqueComercial.DatosDuplicados",
           description: "Ya existe un bloque comercial con los datos proporcionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
          code: "BloqueComercial.DatosDuplicadosArchivo",
          description: "Ya existe un bloque comercial con los datos proporcionados en en el archivo");

        public readonly static Error NombreInvalido = Error.Validation(
          code: "BloqueComercial.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

    }
}
