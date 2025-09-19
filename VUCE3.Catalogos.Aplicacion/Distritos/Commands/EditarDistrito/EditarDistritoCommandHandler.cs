using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.EditarDistrito
{
    public class EditarDistritoCommandHandler : IRequestHandler<EditarDistritoCommand, ErrorOr<Tuple<Distrito, Distrito>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarDistritoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Distrito, Distrito>>> Handle(EditarDistritoCommand command, CancellationToken cancellationToken)
        {
            // Obtener distrito por Id
            var distrito = await _unitOfWork.DistritosRepository.ObtenerDistritoPorId(command.IdDistrito);
            if (distrito.IsError)
            {
                return distrito.Errors;
            }

            if (command.ListaCambios.Contains("IdCanton"))
            {
                return ErroresDistrito.DistritoCantonNoActualizable;
            }

            //Valida tamaño nombre
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre50(command.Distrito.Nombre))
            {
                return ErroresDistrito.DistritoNombreInvalido;
            }

            //Valida tamaño nombre
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo5(command.Distrito.Codigo))
            {
                return ErroresDistrito.DistritoCodigoInvalido;
            }

            var comprobarCodigo = distrito.Value.Codigo;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Distrito.Codigo;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.DistritosRepository.ValidarDistrito(command.IdDistrito, comprobarCodigo, 0);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresDistrito.DistritoDatosDuplicados;
                }
            }

            //se obtiene copia de la distrito antes de aplicar cambios
            Distrito distritoAntes = JsonConvert.DeserializeObject<Distrito>(
                JsonConvert.SerializeObject(distrito.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? distrito.Value;

            var result = await _unitOfWork.DistritosRepository.ActualizarDistrito(command.Distrito, command.IdDistrito, command.ListaCambios);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(distritoAntes, distrito.Value);
        }
    }
}
