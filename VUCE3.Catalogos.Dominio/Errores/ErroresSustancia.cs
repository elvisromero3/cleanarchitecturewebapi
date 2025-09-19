using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresSustancia
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Sustancia.NoEncontrada",
            description: "Sustancia no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "Sustancia.DatosDuplicados",
           description: "Ya existe una sustancia con los datos proporcionados");

        public readonly static Error SustanciaNombreInvalido = Error.Validation(
         code: "Sustancia.NombreInvalido",
         description: "El nombre es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 300 caracteres");

        public readonly static Error SustanciasCasInvalido = Error.Validation(
         code: "Sustancia.CasInvalido",
         description: "El número CAS es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error SustanciaListaCaqInvalido = Error.Validation(
         code: "Sustancia.ListaCaqInvalido",
         description: "La lista CAQ es un campo requerido, debe tener un tamaño mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
          code: "Sustancia.DatosDuplicadosArchivo",
          description: "Ya existe una sustancia con los datos proporcionadosen en el archivo");
                
    }
}