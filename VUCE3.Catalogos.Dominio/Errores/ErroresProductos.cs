using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresProductos
    {

        public readonly static Error NoEncontrado = Error.NotFound(
        code: "Productos.NoEncontrado",
        description: "Producto no encontrado");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Productos.DatosDuplicados",
           description: "Ya existe un Producto con los datos proporcionados");

        public readonly static Error NombreComunInvalido = Error.Validation(
          code: "Productos.NombreComunInvalido",
          description: "El nombre común es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error TamanoClase = Error.Validation(
            code: "Productos.TamanoClase",
            description: "La clase es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 20 caracteres");

        public readonly static Error TamanoPresentacion = Error.Validation(
            code: "Productos.TamanoPresentacion",
            description: "La presentación es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 20 caracteres");

        public readonly static Error TamanoNombreCientifico = Error.Validation(
            code: "Productos.TamanoNombreCientifico",
            description: "El nombre científico es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 50 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Productos.DatosDuplicadosArchivos",
            description: "El archivo contiene datos duplicados");
    }
}
