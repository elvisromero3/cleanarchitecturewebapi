using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresTarifa
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Tarifa.NoEncontrada",
            description: "Tarifa no encontrada");
    }
}
