using ErrorOr;
namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresProfesionales
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Regente.NoEncontrado",
            description: "Regente no encontrado");

        public readonly static Error NoEncontradas = Error.NotFound(
            code: "Regentes.NoEncontrados",
            description: "Regentes no encontrados");

        public readonly static Error DatosDuplicados = Error.Validation(
            code: "Regente.DatosDuplicados",
            description: "Ya existe un regente con los datos proporcionados");

        public readonly static Error FalloServicioAccesoGestionUsuario = Error.Failure(
            code: "Regente.FalloServicioAccesoGestionUsuario",
            description: "Falló al obtener Instituciones");

        public readonly static Error InstitucionNoEncontrado = Error.NotFound(
           code: "Regente.IntitucionNoEncontrado",
           description: "Institución no encontrada");

        public readonly static Error TipoIdentificacionIncorrecta = Error.Validation(
            code: "Regente.TipoIdentificacionIncorrecta",
            description: "Regente con tipo de identificación incorrecta");

        public readonly static Error EmailInvalido = Error.NotFound(
            code: "Regente.EmailInvalido",
            description: "Email inválido");

        public readonly static Error TamanoEmail = Error.Conflict(
            code: "Regente.TamanoEmail",
            description: "El email debe tener máximo 100 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Regente.DatosDuplicadosArchivo",
            description: "Profesionales contiene datos duplicados");

        public readonly static Error TamanoNroIdentificacion = Error.Validation(
            code: "Regente.TamanoNroIdentificacion",
            description: "El tamaño del número de identificación es incorrecto");

        public readonly static Error TamanoNombre = Error.Validation(
            code: "Regente.TamanoNombre",
            description: "El nombre es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error TamanoProfesion = Error.Validation(
            code: "Regente.TamanoProfesion",
            description: "La profesión es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error TamanoCodigoRegente = Error.Validation(
            code: "Regente.TamanoCodigoRegente",
            description: "El código de regente debe tener un tamaño mayor a 0 y un máximo de 20 caracteres");

        public readonly static Error IdInstitucionInvalido = Error.Validation(
            code: "Regente.IdInstitucionInvalido",
            description: "El identificador de institución es un campo requerido y su valor debe ser mayor a 0");

        public readonly static Error TipoIdentificacionFisicaComienza0 = Error.Validation(
            code: "TipoIdentificacionFisicaComienza0",
            description: "El número de identificación de tipo física no puede comenzar con 0");
    }
}
