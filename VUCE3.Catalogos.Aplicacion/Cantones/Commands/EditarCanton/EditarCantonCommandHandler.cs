using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cantones.Commands.EditarCanton
{
    public class EditarCantonCommandHandler : IRequestHandler<EditarCantonCommand, ErrorOr<Tuple<Dominio.Entidades.Canton, Dominio.Entidades.Canton>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarCantonCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Canton, Dominio.Entidades.Canton>>> Handle(EditarCantonCommand command, CancellationToken cancellationToken)
        {
            if (command.ListaCambios.Contains("IdProvincia"))
            {
                return ErroresCanton.CantonProvinciaNoActualizable;
            }

            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo3(command.Canton.Codigo))
            {
                return ErroresCanton.CantonCodigoInvalido;
            }

            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre50(command.Canton.Nombre))
            {
                return ErroresCanton.NombreInvalido;
            }

            // Obtener canton por Id
            var canton = await _unitOfWork.CantonesRepository.ObtenerCantonesPorId(command.IdCanton);
            if (canton.IsError)
            {
                return canton.Errors;
            }

            var comprobarCodigo = canton.Value.Codigo;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Canton.Codigo;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.CantonesRepository.ValidarCantones(command.IdCanton, comprobarCodigo, 0);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCanton.DatosDuplicados;
                }
            }            

            //se obtiene copia de la canton antes de aplicar cambios
            Dominio.Entidades.Canton cantonAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Canton>(
                JsonConvert.SerializeObject(canton.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? canton.Value;

            var result = await _unitOfWork.CantonesRepository.ActualizarCantones(command.IdCanton, command.ListaCambios, command.Canton);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(cantonAntes, canton.Value);
        }
    }
}
