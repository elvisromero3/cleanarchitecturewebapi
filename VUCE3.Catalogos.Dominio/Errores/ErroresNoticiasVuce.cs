using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresNoticiasVuce
    {
        public readonly static Error NoEncontrada = Error.NotFound(
             code: "NoticiasVuce.NoEncontrada",
             description: "NoticiasVuce no encontrada");

        public readonly static Error DatosDuplicados = Error.Validation(
           code: "NoticiasVuce.DatosDuplicados",
           description: "NoticiasVuce contiene datos duplicados");

        public readonly static Error DatosDuplicadosArchivo = Error.Validation(
            code: "NoticiasVuce.DatosDuplicadosArchivo",
            description: "NoticiasVuce contiene datos duplicados");

        public readonly static Error NoticiasLinkExcedeLimite = Error.Validation(
           code: "ValidacionNoticiasLink",
           description: "El enlace debe tener máximo 250 caracteres");

        public readonly static Error NoticiasLinkInValido = Error.Validation(
           code: "NoticiasVuce.NoticiasLinkInvalido",
           description: "El enlace es un link invalido");

        public readonly static Error NoticiasVuceTituloInvalido = Error.Validation(
           code: "NoticiasVuce.TituloInvalido",
           description: "El título de noticias VUCE es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error NoticiasVuceTituloInglesInvalido = Error.Validation(
           code: "NoticiasVuce.TituloInglesInvalido",
           description: "El título en ingles de noticias VUCE es un campo requerido, su tamaño debe ser mayor a 0 y un máximo de 100 caracteres");

        public readonly static Error NoticiasVuceTextoInvalido = Error.Validation(
           code: "NoticiasVuce.TextoInvalido",
           description: "El texto de noticias VUCE es un campo requerido y su tamaño debe ser mayor a 0");

        public readonly static Error NoticiasVuceTextoInglesInvalido = Error.Validation(
           code: "NoticiasVuce.TextoInglesInvalido",
           description: "El texto en inglés de noticias VUCE es un campo requerido y su tamaño debe ser mayor a 0");
       
    }
}
