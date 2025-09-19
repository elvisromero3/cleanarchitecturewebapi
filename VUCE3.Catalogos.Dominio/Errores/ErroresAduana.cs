using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresAduana
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Aduana.NoEncontrada",
            description: "Aduana no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
            code: "Aduana.DatosDuplicados",
            description: "Ya existe una aduana con los datos proporcionados");

        public readonly static Error FalloServicioAccesoGestionUsuario = Error.Failure(
            code: "Aduana.FalloServicioAccesoGestionUsuario",
            description: "Fallo al obtener InstitucionesAutorizadas");

        public readonly static Error ExisteInstitucionesAutorizadas = Error.Conflict(
            code: "Aduana.ExisteInstitucionesAutorizadas",
            description: "La aduana se encuentra asignada a una institucion autorizada");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "Aduana.DatosDuplicadosArchivo",
            description: "Aduana contiene datos duplicados");

        public readonly static Error AduanaNombreInvalido = Error.Validation(
          code: "Aduana.NombreInvalido",
          description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");
    }
}
