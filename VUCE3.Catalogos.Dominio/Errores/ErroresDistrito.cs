using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresDistrito
    {
        public readonly static Error DistritoNoEncontrado = Error.NotFound(
           code: "Distrito.NoEncontrado",
           description: "Distrito no encontrado");

        public readonly static Error DistritoDatosDuplicados = Error.Conflict(
           code: "Distrito.DatosDuplicados",
           description: "Ya existe un distrito con los datos proporcionados");

        public readonly static Error DistritoNombreInvalido = Error.Validation(
          code: "Distrito.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error DistritoCantonNoActualizable = Error.Validation(
         code: "Distrito.CantonNoActualizable",
         description: "El campo cantón no es actualizable");

        public readonly static Error BarriosRelacionados = Error.Validation(
           code: "Distrito.BarriosRelacionados",
           description: "Existe distrito con barrios relacionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
         code: "Distrito.DatosDuplicadosArchivo",
         description: "Existen datos duplicados en el fichero");

        public readonly static Error CantonInexistente = Error.Validation(
          code: "Distrito.CantonInexistente",
          description: "Cantón inexistente");

        public readonly static Error ProvinciaInexistente = Error.Validation(
          code: "Distrito.ProvinciaInexistente",
          description: "Provincia inexistente");

        public readonly static Error DistritoCodigoInvalido = Error.Validation(
          code: "Distrito.CodigoInvalido",
          description: "El código es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 5 caracteres");
    }
}
