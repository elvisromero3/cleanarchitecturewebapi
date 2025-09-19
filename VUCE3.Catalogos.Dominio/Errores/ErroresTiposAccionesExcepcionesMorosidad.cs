using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresTiposAccionesExcepcionesMorosidad
    {
        public readonly static Error TipoAccionExcepcionMorosidadNoEncontrado = Error.NotFound(
        code: "TipoAccionExcepcionMorosidad.NoEncontrado",
        description: "Tipo acción excepción morosidad no encontrado");
    }
}
