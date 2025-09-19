using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresVariedad
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Variedad.NoEncontrada",
            description: "Variedad no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
            code: "Variedad.DatosDuplicados",
            description: "Ya existe una variedad con los datos proporcionados");

        public readonly static Error VariedadRelacionCultivo = Error.Conflict(
            code: "Variedad.VariedadRelacionCultivo",
            description: "La variedad que desea eliminar está asociada a un cultivo");

        public readonly static Error VariedadesRelacionCultivo = Error.Conflict(
            code: "Variedad.VariedadesRelacionCultivo",
            description: "Al menos una variedad que desea eliminar está asociada a un cultivo");

        public readonly static Error VariedadLongitudNombre = Error.Validation(
            code: "Variedad.VariedadLongitudNombre",
            description: "La longitud del nombre de la variedad debe ser entre 1 y 100 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Variedad.DatosDuplicadosArchivo",
            description: "Variedades contiene datos duplicados");
               
        public readonly static Error VariedadNombreInvalido = Error.Validation(
          code: "Variedad.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error VariedadCodigoInvalido = Error.Validation(
          code: "Variedad.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 17 caracteres");
    }
}
