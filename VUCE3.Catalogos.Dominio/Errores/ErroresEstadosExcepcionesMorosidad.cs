using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static  class ErroresEstadosExcepcionesMorosidad
    {
        public readonly static Error EstadoExcepcionMorosidadNoEncontrado = Error.NotFound(
        code: "EstadoExcepcionMorosidad.NoEncontrado",
        description: "Estado excepción morosidad no encontrado");
    }
}
