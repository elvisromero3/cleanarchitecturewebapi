using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCategoria
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Categoria.NoEncontrada",
            description: "Categoria no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
            code: "Categoria.DatosDuplicados",
            description: "Ya existe una categoría con los datos proporcionados");

        public readonly static Error NombreInvalido = Error.Validation(
            code: "Categoria.NombreInvalido",
            description: "La categoría de mercancía es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");
       
        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Categoria.DatosDuplicadosArchivo",
            description: "Categoria contiene datos duplicados");

        public readonly static Error CategoriaInstitucionInvalida = Error.NotFound(
          code: "Categoria.CategoriaInstitucionInvalida",
          description: "Institución no es DCA o DIPOA");

        public readonly static Error TipoProductoRelacionado = Error.Validation(
           code: "Categoria.TipoProductoRelacionado",
           description: "Existe caracteristica de tipo producto relacionadas");

        public readonly static Error ProductoRequisitoRelacionado = Error.Validation(
         code: "Categoria.ProductoRequisitoRelacionado",
         description: "Existe un producto requisito relacionado");

        public readonly static Error InstitucionNoEncontrado = Error.NotFound(
        code: "Categoria.InstitucionNoEncontrado",
        description: "Institución no encontrada");

        public readonly static Error FalloServicioAccesoGestionUsuario = Error.Failure(
            code: "Categoria.FalloServicioAccesoGestionUsuario",
            description: "Falló al obtener Instituciones");
    }
}
