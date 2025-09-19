using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresEmpresas
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Empresa.NoEncontrada",
            description: "Empresa no encontrada");

        public readonly static Error NoEncontradas = Error.NotFound(
            code: "Empresas.NoEncontradas",
            description: "Empresas no encontradas");

        public readonly static Error FalloServicioRegisto = Error.Failure(
            code: "Empresas.FalloServicioRegistro",
            description: "Fallo al obtener Tipo Identificadores");

        public readonly static Error TipoIdentificacionNoEncontrado = Error.NotFound(
           code: "Empresas.tipoIdentificacionNoEncontrado",
           description: "Tipo Identificacion no encontrado");

        public readonly static Error DatosDuplicados = Error.Validation(
          code: "Empresas.DatosDuplicados",
          description: "Empresa asociada al regente ya existe");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Empresas.DatosDuplicadosArchivo",
            description: "Empresas contiene datos duplicados");

        public readonly static Error EmpresasCodigoExcedeLimite = Error.Validation(
            code: "Empresas.CodigoExcedeLimite",
            description: "El código es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 12 caracteres");

        public readonly static Error EmpresaNombreExcedeLimite = Error.Validation(
            code: "Empresa.NombreExcedeLimite",
           description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error NumeroIdentificacionExcedeLimite = Error.Validation(
          code: "ValidacionNumeroIdentificacion",
          description: "El tamaño del número de identificación es incorrecto");

        public readonly static Error EmpresasIdProfesionalInvalido = Error.Validation(
            code: "Empresas.IdProfesionalInvalido",
            description: "El identificador de profesional es un campo requerido y su valor debe ser mayor a 0");

        public readonly static Error TipoIdentificacionFisicaComienza0 = Error.Validation(
            code: "TipoIdentificacionFisicaComienza0",
            description: "El número de identificación de tipo física no puede comenzar con 0");



    }
}
