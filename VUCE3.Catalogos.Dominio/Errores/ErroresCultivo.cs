using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCultivo
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Cultivo.NoEncontrado",
            description: "Cultivo no encontrado");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Cultivo.DatosDuplicados",
           description: "Ya existe un cultivo con los datos proporcionados");
                
        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Cultivo.DatosDuplicadosArchivo",
            description: "Cultivo contiene datos duplicados");

        public readonly static Error IdVarierdadExcedeLimite = Error.Validation(
          code: "ValidacionIdVariedad",
          description: "El id variedad debe estar entre 1 y 999999999");

        public readonly static Error CultivoCodigoInvalido = Error.Validation(
         code: "Cultivo.CodigoInvalido",
         description: "El código es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 25 caracteres");

        public readonly static Error CultivoIdVariedadInvalido = Error.Validation(
          code: "Cultivo.IdVariedadInvalido",
          description: "El identificador de variedad es un campo requerido y su valor debe ser mayor a 0");

        public readonly static Error CultivoNombreCientificoInvalido = Error.Validation(
         code: "Cultivo.NombreCientificoInvalido",
         description: "El nombre cientifico es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error CultivoNombreInvalido = Error.Validation(
         code: "Cultivo.NombreInvalido",
         description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

    }
}
