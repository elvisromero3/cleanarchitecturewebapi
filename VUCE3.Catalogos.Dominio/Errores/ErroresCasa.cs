using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCasa
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Casa.NoEncontrada",
            description: "Casa no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Casa.DatosDuplicados",
           description: "Ya existe una casa con los datos proporcionados");
        
        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
          code: "Casa.DatosDuplicadosArchivo",
          description: "Ya existe una casa con los datos proporcionadosen en el archivo");

        public readonly static Error CasaCodigoInvalido = Error.Validation(
          code: "Casa.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres");

        public readonly static Error CasaNombreInvalido = Error.Validation(
          code: "Casa.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

    }
}
