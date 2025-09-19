using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCanton
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Canton.NoEncontrado",
            description: "Cantón no encontrado");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Canton.DatosDuplicados",
           description: "Ya existe un cantón con los datos proporcionados");

        public readonly static Error DistritosRelacionados = Error.Validation(
           code: "Canton.DistritosRelacionados",
           description: "Existe cantón con distritos relacionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
          code: "Canton.DatosDuplicadosArchivo",
          description: "Existen datos duplicados en el fichero");

        public readonly static Error NombreInvalido = Error.Validation(
          code: "Canton.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error ProvinciaInexistente = Error.Validation(
           code: "Canton.ProvinciaInexistente",
           description: "Provincia inexistente");

        public readonly static Error CantonCodigoInvalido = Error.Validation(
          code: "Canton.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 3 caracteres");

        public readonly static Error CantonProvinciaNoActualizable = Error.Validation(
        code: "Canton.ProvinciaNoActualizable",
        description: "El campo provincia no es actualizable");
    }
}
