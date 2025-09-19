using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.BloqueComercial.Commands.EditarBloqueComercial
{
    public class EditarBloqueComercialCommandHandler : IRequestHandler<EditarBloqueComercialCommand, ErrorOr<Tuple<Dominio.Entidades.BloqueComercial, Dominio.Entidades.BloqueComercial>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarBloqueComercialCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.BloqueComercial, Dominio.Entidades.BloqueComercial>>> Handle(EditarBloqueComercialCommand command, CancellationToken cancellationToken)
        {
            // Obtener bloquecomercial por Id
            var bloquecomercial = await _unitOfWork.BloqueComercialRepository.ObtenerBloqueComercialPorId(command.IdBloqueComercial);
            if (bloquecomercial.IsError)
            {
                return bloquecomercial.Errors;
            }

            var comprobarNombre = bloquecomercial.Value.Nombre;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Nombre"))
            {
                if (Validadores.LongitudMaximaNoNull(command.BloqueComercial.Nombre, 300))
                {
                    return ErroresBloqueComercial.NombreInvalido;
                }

                comprobarNombre = command.BloqueComercial.Nombre;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.BloqueComercialRepository.ValidarBloqueComercial(command.IdBloqueComercial, comprobarNombre);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresBloqueComercial.DatosDuplicados;
                }
            }

            //se obtiene copia de la bloquecomercial antes de aplicar cambios
            Dominio.Entidades.BloqueComercial bloquecomercialAntes = JsonConvert.DeserializeObject<Dominio.Entidades.BloqueComercial>(
                JsonConvert.SerializeObject(bloquecomercial.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? bloquecomercial.Value;

            var result = await _unitOfWork.BloqueComercialRepository.ActualizarBloqueComercial(command.IdBloqueComercial, command.ListaCambios, command.BloqueComercial);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(bloquecomercialAntes, bloquecomercial.Value);
        }
    }
}
