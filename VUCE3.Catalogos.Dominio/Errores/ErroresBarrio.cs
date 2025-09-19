using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresBarrio
    {
        public readonly static Error BarrioNoEncontrado = Error.NotFound(
           code: "Barrio.NoEncontrado",
           description: "Barrio no encontrado");

        public readonly static Error BarrioDatosDuplicados = Error.Conflict(
           code: "Barrio.DatosDuplicados",
           description: "Ya existe un barrio con los datos proporcionados");

        public readonly static Error BarrioNombreInvalido = Error.Validation(
          code: "Barrio.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error BarrioDistritoNoActualizable = Error.Validation(
         code: "Barrio.DistritoNoActualizable",
         description: "El campo distrito no es actualizable");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
        code: "Barrio.DatosDuplicadosArchivo",
        description: "Existen datos duplicados en el fichero");

        public readonly static Error DistritoInexistente = Error.Validation(
          code: "Barrio.DistritoInexistente",
          description: "Distrito inexistente");

        public readonly static Error CantonInexistente = Error.Validation(
          code: "Barrio.CantonInexistente",
          description: "Cantón inexistente");

        public readonly static Error ProvinciaInexistente = Error.Validation(
          code: "Barrio.ProvinciaInexistente",
          description: "Provincia inexistente");

        public readonly static Error BarrioCodigoInvalido = Error.Validation(
          code: "Barrio.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 7 caracteres");
    }
}
