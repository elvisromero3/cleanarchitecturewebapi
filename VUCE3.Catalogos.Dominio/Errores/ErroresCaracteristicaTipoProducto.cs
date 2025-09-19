using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCaracteristicaTipoProducto
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "CaracteristicaTipoProducto.NoEncontrada",
            description: "Característica tipo producto no encontrada");

        public readonly static Error CaracteristicaNoEncontrada = Error.NotFound(
           code: "CaracteristicaTipoProducto.CaracteristicaNoEncontrada",
           description: "Característica no encontrada");

        public readonly static Error CaracteristicaNoPerteneceInstitucion = Error.NotFound(
          code: "CaracteristicaTipoProducto.CaracteristicaNoPerteneceInstitucion",
          description: "La característica no pertenece a la institución");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "CaracteristicaTipoProducto.DatosDuplicados",
           description: "Ya existe una característica tipo producto con los datos proporcionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "CaracteristicaTipoProducto.DatosDuplicadosArchivo",
            description: "Existen datos duplicado en el archivo");
    }
}
