using ErrorOr;


namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresExcepcionMorosidad
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "ExcepcionMorosidad.NoEncontrada",
            description: "Excepción morosidad no encontrada");
        public readonly static Error TipoIdentificacionNoEncontrado = Error.NotFound(
            code: "ExcepcionMorosidad.TipoIdentificacionNoEncontrado",
            description: "Tipo Identificación no encontrado");
        public readonly static Error TipoTramiteInvalido = Error.NotFound(
            code: "ExcepcionMorosidad.TipoTramiteInvalido",
            description: "Tipo Tramite inválido"); 
        public readonly static Error SubTipoTramiteInvalido = Error.NotFound(
            code: "ExcepcionMorosidad.SubTipoTramiteInvalido",
            description: "Sub Tipo Tramite inválido");
        public readonly static Error TipoAccionInvalido = Error.NotFound(
            code: "ExcepcionMorosidad.TipoAccionInvalido",
            description: "Tipo Accion inválido");
        public readonly static Error NumeroIdentificacionExcedeLimite = Error.Validation(
            code: "ExcepcionMorosidad.NumeroIdentificacionExcedeLimite",
            description: "El tamaño del número de identificación es incorrecto");
        public readonly static Error TipoIdentificacionFisicaComienza0 = Error.Validation(
            code: "ExcepcionMorosidad.TipoIdentificacionFisicaComienza0",
            description: "El número de identificación de tipo física no puede comenzar con 0");
        public readonly static Error FechaInicioMayorIgualFechaVencimiento = Error.Validation(
            code: "ExcepcionMorosidad.FechaInicioMayorIgualFechaVencimiento",
            description: "La fecha de inicio debe ser menor a la fecha de vencimiento");
        public readonly static Error FechaInicioMenorAlDia = Error.Validation(
            code: "ExcepcionMorosidad.FechaInicioMenorAlDia",
            description: "La fecha de inicio es un campo requerido, debe ser una fecha mayor o igual a hoy");
        public readonly static Error EliminacionNoPermitida = Error.Validation(
            code: "ExcepcionMorosidad.EliminacionNoPermitida",
            description: "Eliminación no permitida por estado incorrecto");
        public readonly static Error NombreEmpresaInvalido = Error.Validation(
            code: "ExcepcionMorosidad.NombreEmpresaInvalido",
            description: "El nombre de empresa es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");
        public readonly static Error ObservacionesInvalidas = Error.Validation(
            code: "ExcepcionMorosidad.ObservacionesInvalidas",
            description: "El campo observaciones es requerido, debe tener un tamaño mayor a 0 y un máximo de 250 caracteres");
        public readonly static Error TipoTramiteNTRegimen = Error.Validation(
            code: "ExcepcionMorosidad.TipoTramiteNTRegimen",
            description: "Si el tipo de trámite es nota técnica es obligatorio el ingreso del régimen");
        public readonly static Error TipoTramiteRegimen = Error.Validation(
            code: "ExcepcionMorosidad.TipoTramiteRegimen",
            description: "Si el tipo de trámite es distinto a nota técnica no se debe ingresar el régimen");
        public readonly static Error ExcepcionMorosidadDatosDuplicados = Error.Validation(
            code: "ExcepcionMorosidad.DatosDuplicados",
            description: "Ya existe una excepción de morosidad con los datos proporcionados");
        public readonly static Error FechaVencimientoInvalida = Error.Validation(
            code: "ExcepcionMorosidad.FechaVencimientoInvalida",
            description: "La fecha de vencimiento debe ser una fecha mayor o igual a hoy");

        public readonly static Error FechaInicioInvalida = Error.Validation(
            code: "ExcepcionMorosidad.FechaInicioInvalida",
            description: "Fecha de inicio inválida, es un campo requerido y debe ser una fecha mayor o igual a hoy");

        public readonly static Error DatosArchivoImportarInvalido = Error.Validation(
            code: "ExcepcionMorosidad.DatosArchivoImportarInvalido",
            description: "Importe un archivo válido para continuar");

        public readonly static Error FalloServicioTramites = Error.Failure(
            code: "ExcepcionMorosidad.FalloServicioTramites",
            description: "Fallo al consultar el servicio de trámites");

    }
}
