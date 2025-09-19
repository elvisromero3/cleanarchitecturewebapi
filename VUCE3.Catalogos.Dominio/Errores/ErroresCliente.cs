using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCliente
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Cliente.NoEncontrado",
            description: "Cliente no encontrado");

        public readonly static Error FalloServicioRegisto = Error.Failure(
            code: "Cliente.FalloServicioRegistro",
            description: "Fallo al obtener Tipo Identificadores");

        public readonly static Error TipoIdentificacionNoEncontrado = Error.NotFound(
            code: "Cliente.tipoIdentificacionNoEncontrado",
            description: "Tipo Identificacion no encontrado");

        public readonly static Error ClienteExiste = Error.Validation(
            code: "Cliente.Existe",
            description: "Ya existe un cliente con los datos proporcionados");

        public readonly static Error DatosDuplicados = Error.Validation(
            code: "Cliente.DatosDuplicados",
            description: "Cliente contiene datos duplicados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Cliente.DatosDuplicadosArchivo",
            description: "Cliente contiene datos duplicados");

        public readonly static Error TamanoNroIdentificacion = Error.Conflict(
            code: "Cliente.TamanoNroIdentificacion",
            description: "El tamaño del número de identificación es incorrecto");
                
        public readonly static Error ClienteTamanoNombreCliente = Error.Conflict(
            code: "Cliente.TamanoNombreCliente",
            description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error ClienteTamanoCodigoCliente = Error.Conflict(
            code: "Cliente.TamanoCodigoCliente",
            description: "El código de cliente es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 35 caracteres");

       public readonly static Error NumeroIdentificacionExcedeLimite = Error.Validation(
          code: "ValidacionNumeroIdentificacion",
          description: "El número de identificación debe tener máximo 12 caracteres");

        public readonly static Error FechaVencimientoMenorHoy = Error.Validation(
          code: "Cliente.FechaVencimientoMenorHoy",
          description: "La fecha de vencimiento debe ser mayor al día de hoy");

        public readonly static Error TipoIdentificacionFisicaComienza0 = Error.Validation(
            code: "TipoIdentificacionFisicaComienza0",
            description: "El número de identificación de tipo física no puede comenzar con 0");
    }    
}
