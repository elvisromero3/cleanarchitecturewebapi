using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresSector
    {
        public readonly static Error SectorNoEncontrado = Error.NotFound(
           code: "Sector.NoEncontrado",
           description: "Sector no encontrado");

        public readonly static Error SectorDatosDuplicados = Error.Conflict(
           code: "Sector.DatosDuplicados",
           description: "Ya existe un sector con los datos proporcionados");
        
        public readonly static Error SectorNombreInvalido = Error.Validation(
          code: "Sector.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Sector.DatosDuplicadosArchivos",
            description: "El archivo contiene datos duplicados");

        public readonly static Error DatosDuplicados = Error.Validation(
            code: "Sector.DatosDuplicados",
            description: "Ya existe un sector con los datos suministrados");

        public readonly static Error SectorCodigoInvalido = Error.Validation(
       code: "Sector.CodigoInvalido",
       description: "El codigo es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 10 caracteres");

    }
}
