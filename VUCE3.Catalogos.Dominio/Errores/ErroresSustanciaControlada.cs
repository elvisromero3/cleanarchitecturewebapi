using ErrorOr;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresSustanciaControlada
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "SustanciaControlada.NoEncontrada",
            description: "Sustancia controlada no encontrada");
        public readonly static Error FamiliaNoEncontrada = Error.NotFound(
          code: "SustanciaControlada.FamiliaNoEncontrada",
          description: "Familia no encontrada");

        public readonly static Error DatosDuplicados = Error.Conflict(
           code: "SustanciaControlada.DatosDuplicados",
           description: "Ya existe una sustancia controlada con los datos proporcionados");

        public readonly static Error DatosDuplicadosArchivo = Error.Conflict(
         code: "SustanciaControlada.DatosDuplicadosArchivo",
         description: "Ya existe una sustancia controlada con los datos proporcionados");

        public readonly static Error SustanciaControladaClasificacionArancelariaTamano = Error.Conflict(
         code: "SustanciaControlada.ClasificacionArancelariaTamano",
         description: "El tamaño de clasificacion arancelaria es incorrecto");

        public readonly static Error SustanciaControladaClasificacionAshraeTamano = Error.Conflict(
         code: "SustanciaControlada.ClasificacionAshraeTamano",
         description: "El tamaño de clasificacion ashrae es incorrecto");

        public readonly static Error SustanciaControladaPotencialCalentamientoGlobalTamano = Error.Conflict(
        code: "SustanciaControlada.PotencialCalentamientoGlobalTamano",
        description: "El tamaño de potencial calentamiento global es incorrecto");
        public readonly static Error SustanciaControladaTipoGasTamano = Error.Conflict(
       code: "SustanciaControlada.TipoGasTamano",
       description: "El tamaño del tipo de gas es incorrecto");
    }
}
