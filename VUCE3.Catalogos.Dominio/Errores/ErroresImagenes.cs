using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresImagenes
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Imagen.NoEncontrada",
            description: "Imagen no encontrada");
    }
}
