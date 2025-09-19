using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresCatalogo
    {
        public readonly static Error NoEncontrado = Error.NotFound(
            code: "Catalogo.NoEncontrado",
            description: "Catálogo no encontrado");
    }
}
