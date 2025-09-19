using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCaracteristica
    {
        public readonly static Error CaracteristicaNoEncontrada = Error.NotFound(
           code: "Caracteristica.NoEncontrada",
           description: "Característica no encontrada");

        public readonly static Error CaracteristicaDatosDuplicados = Error.Conflict(
            code: "Caracteristica.DatosDuplicados",
            description: "Ya existe una característica con los datos proporcionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
            code: "Caracteristica.DatosDuplicadosArchivo",
            description: "Existen datos duplicados en el archivo importado");
       
        public readonly static Error CaracteristicaNombreInvalido = Error.Validation(
          code: "Caracteristica.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 150 caracteres");

        public readonly static Error CaracteristicaFalloServicioAccesoGestionUsuario = Error.Failure(
            code: "Caracteristica.FalloServicioAccesoGestionUsuario",
            description: "Falló al obtener Instituciones");

        public readonly static Error CaracteristicaInstitucionNoEncontrada = Error.NotFound(
          code: "Caracteristica.IntitucionNoEncontrada",
          description: "Institución no encontrada");

        public readonly static Error CaracteristicaInstitucionInvalida = Error.NotFound(
          code: "Caracteristica.CaracteristicaInstitucionInvalida",
          description: "Institución no es DCA o DIPOA");

        public readonly static Error TipoProductoRelacionado = Error.Validation(
           code: "Caracteristica.TipoProductoRelacionado",
           description: "Existe caracteristica de tipo producto relacionadas");

    }
}
