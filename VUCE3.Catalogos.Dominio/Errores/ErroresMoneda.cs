using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUCE3.Catalogos.Dominio.Errores
{
    public static class ErroresMoneda
    {
        public readonly static Error NoEncontrada = Error.NotFound(
            code: "Moneda.NoEncontrada",
            description: "Moneda no encontrada");
    }
}
