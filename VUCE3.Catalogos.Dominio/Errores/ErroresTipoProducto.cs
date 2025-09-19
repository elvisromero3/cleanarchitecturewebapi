using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresTipoProducto
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "TipoProducto.NoEncontrado",
            description: "Tipo producto no encontrado");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "TipoProducto.DatosDuplicados",
           description: "Ya existe un tipo producto con los datos proporcionados");

        public readonly static Error FalloServicioAccesoGestionUsuario = Error.Failure(
            code: "TipoProducto.FalloServicioAccesoGestionUsuario",
            description: "Falló al obtener Instituciones");

        public readonly static Error InstitucionNoEncontrado = Error.NotFound(
          code: "TipoProducto.InstitucionNoEncontrado",
          description: "Institución no encontrada");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
           code: "TipoProducto.DatosDuplicadosArchivo",
           description: "Ya existe un tipo de producto con los datos proporcionados");

        public readonly static Error CaracteristicaRelacionadas = Error.Validation(
           code: "TipoProducto.CaracteristicaRelacionadas",
           description: "Existe caracteristica con tipo de productos relacionadas");

        public readonly static Error TipoTamano = Error.Conflict(
         code: "TipoProducto.TipoTamano",
         description: "El tamaño de tipo es incorrecto");

        public readonly static Error InstitucionNoValida = Error.Conflict(
         code: "TipoProducto.InstitucionNoValida",
         description: "Institucion no valida");
        
        public readonly static Error ProductoRequisitoExiste = Error.Conflict(
         code: "TipoProducto.ProductoRequisitoExiste",
         description: "Producto requisito con tipo producto asociado");

        public readonly static Error CategoriaNoEncontrado = Error.NotFound(
         code: "TipoProducto.CategoriaNoEncontrado",
         description: "Categoria no encontrada");

        public readonly static Error CategoriaPerteneceAOtraInstitucion = Error.NotFound(
        code: "TipoProducto.CategoriaPerteneceAOtraInstitucion",
        description: "La categoría no pertenece a la institución");

        public readonly static Error CategoriaInstitucion = Error.NotFound(
         code: "TipoProducto.CategoriaInstitucion",
         description: "La categoría no pertenece a la institución");
    }
}
