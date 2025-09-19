using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresRequisito
    {
        public readonly static Error RequisitoNoEncontrado = Error.NotFound(
           code: "Requisito.NoEncontrado",
           description: "Requisito no encontrado");

        public readonly static Error RequisitoDatosDuplicados = Error.Conflict(
           code: "Requisito.DatosDuplicados",
           description: "Ya existe un requisito con los datos proporcionados");

        public readonly static Error RequisitoDescripcionInvalida = Error.Validation(
          code: "Requisito.DescripcionInvalida",
          description: "La descripción es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error RequisitoFalloServicioAccesoGestionUsuario = Error.Failure(
            code: "Requisito.FalloServicioAccesoGestionUsuario",
            description: "Falló al obtener instituciones");

        public readonly static Error RequisitoInstitucionNoEncontrada = Error.NotFound(
          code: "Requisito.IntitucionNoEncontrada",
          description: "Institución no encontrada");

        public readonly static Error RequisitoPaisNoEncontrado = Error.NotFound(
          code: "Requisito.PaisNoEncontrado",
          description: "Pais no encontrado");

        public readonly static Error RequisitoDatosDuplicadosArchivo = Error.Conflict(
           code: "Requisito.DatosDuplicadosArchivo",
           description: "Ya existe un requisito con los datos proporcionados");

        public readonly static Error RequisitoCodigoTamano = Error.Conflict(
          code: "Requisito.RequisitoCodigoTamano",
          description: "El tamaño de codigo es incorrecto");

        public readonly static Error RequisitoDescripcionTamano = Error.Conflict(
          code: "Requisito.RequisitoDescripcionTamano",
          description: "El tamaño de la descripcion es incorrecto");

        public readonly static Error RequisitoVersionTamano = Error.Conflict(
          code: "Requisito.RequisitoVersionTamano",
          description: "El tamaño de la version es incorrecto");

        public readonly static Error RequisitoInstitucionInvalida = Error.Conflict(
         code: "Requisito.RequisitoInstitucionInvalida",
         description: "Institucion invalida");

        public readonly static Error RequisitoTieneProductosAsociados = Error.Conflict(
         code: "Requisito.RequisitoTieneProductosAsociados",
         description: "El requisito tiene productos asociados");

        public readonly static Error RequisitoImagenObligatoria = Error.Conflict(
         code: "Requisito.RequisitoImagenObligatoria",
         description: "La imagen es un campo requerido, debe tener un tamaño mayor a 0");

        public readonly static Error RequisitoNombreImagen = Error.Conflict(
         code: "Requisito.RequisitoNombreImagen",
         description: "El nombre de la imagen es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");
    }
}
