using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresEstablecimiento
    {
        public static readonly Error NoEncontrado = Error.NotFound(
            code: "Establecimiento.NoEncontrado",
            description: "Establecimiento no encontrado");

        public static readonly Error NumeroCvoInvalido = Error.Conflict(
            code: "Establecimiento.NumeroCvoInvalido",
            description: "El campo NumeroCvo es obligatorio y no debe tener mas de 15 caracteres");

        public static readonly Error NombreEstablecimientoInvalido = Error.Conflict(
            code: "Establecimiento.NombreEstablecimientoInvalido",
            description: "El nombre del establecimiento es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public static readonly Error ActividadPrimariaInvalido = Error.Conflict(
            code: "Establecimiento.ActividadPrimariaInvalido",
            description: "El campo actividad primaria es obligatorio y no debe tener más de 50 caracteres");

        public static readonly Error ActividadSecundariaInvalido = Error.Conflict(
            code: "Establecimiento.ActividadSecundariaInvalido",
            description: "El campo actividad secundaria no debe tener mas de 50 caracteres");

        public static readonly Error ProvinciaInvalido = Error.Conflict(
            code: "Establecimiento.ProvinciaInvalido",
            description: "El campo Provincia es obligatorio y no debe tener mas de 50 caracteres");

        public static readonly Error CantonInvalido = Error.Conflict(
            code: "Establecimiento.CantonInvalido",
            description: "El campo Canton es obligatorio y no debe tener mas de 50 caracteres");

        public static readonly Error DistritoInvalido = Error.Conflict(
            code: "Establecimiento.DistritoInvalido",
            description: "El campo Distrito es obligatorio y no debe tener mas de 50 caracteres");

        public static readonly Error DireccionExactaInvalido = Error.Conflict(
            code: "Establecimiento.DireccionExactaInvalido",
            description: "El campo DireccionExacta es obligatorio y no debe tener mas de 250 caracteres");

        public static readonly Error FechaVencimientoMenorHoy = Error.Conflict(
            code: "Establecimiento.FechaVencimientoMenorHoy",
            description: "La fecha de vencimiento es un campo requerido, debe ser una fecha mayor a la fecha actual");

        public static readonly Error EstablecimientoExiste = Error.NotFound(
            code: "Establecimiento.EstablecimientoExiste",
            description: "El establecimiento ya existe");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
         code: "Establecimiento.DatosDuplicadosArchivo",
         description: "Existen datos duplicados en el fichero");

        public readonly static Error DatosDuplicados = Error.Conflict(
         code: "Establecimiento.DatosDuplicados",
         description: "Ya existe un establecimiento con los datos proporcionados");

        public static readonly Error EstadoEstablecimientoInvalido = Error.Conflict(
           code: "Establecimiento.EstadoEstablecimientoInvalido",
           description: "El campo Estado Establecimiento es obligatorio y no debe tener mas de 20 caracteres");              
    }
}
