using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresProductoRequisito
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "ProductoRequisito.NoEncontrada",
            description: "ProductoRequisito configuracion no encontrada");
        public readonly static Error RequisitoDatosDuplicadosArchivo = Error.Conflict(
          code: "ProductoRequisito.DatosDuplicadosArchivo",
          description: "Ya existe un producto requisito con los datos proporcionados");
        public readonly static Error ProductoRequisitoDuplicados = Error.Conflict(
            code: "ProductoRequisito.Duplicados",
            description: "Ya existe un producto requisito con los datos proporcionados");
        
        public readonly static Error CategoriaNoEncontrada = Error.NotFound(
            code: "ProductoRequisito.CategoriaNoEncontrada",
            description: "ProductoRequisito categroia no encontrada");
        public readonly static Error TipoProductoNoEncontrada = Error.NotFound(
            code: "ProductoRequisito.TipoProductoNoEncontrada",
            description: "ProductoRequisito tipo producto no encontrado");
        public readonly static Error RequisitoNoEncontrado = Error.NotFound(
            code: "ProductoRequisito.RequisitoNoEncontrado",
            description: "Requisito no encontrado");

        public readonly static Error TipoProductoInstitucion = Error.NotFound(
            code: "ProductoRequisito.TipoProductoInstitucion",
            description: "Tipo producto no corresponde al la institución del requisito");
    }
}
