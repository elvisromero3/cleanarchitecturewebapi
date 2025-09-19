using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresProvincia
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Provincia.NoEncontrada",
            description: "Provincia no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Provincia.DatosDuplicados",
           description: "Ya existe una provincia con los datos proporcionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
           code: "Provincia.DatosDuplicadosArchivo",
           description: "Existen datos duplicados en el fichero");

        public readonly static Error ProvinciaNombreInvalido = Error.Validation(
          code: "Provincia.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error ProvinciaCantonesRelacionados = Error.Validation(
           code: "Provincia.CantonesRelacionados",
           description: "La(s) provincia(s) tiene(n) cantones relacionados");

        public readonly static Error ProvinciaCodigoInvalido = Error.Validation(
          code: "Provincia.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 1 caracter");
        
        public readonly static Error ProvinciaRelacionCanton = Error.Validation(
           code: "Provincia.ProvinciaRelacionCanton",
           description: "Existen cantones relacionados con la provincia");
    }
}
