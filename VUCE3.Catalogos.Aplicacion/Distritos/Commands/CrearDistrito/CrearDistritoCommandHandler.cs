using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Distritos.Commands.CrearDistrito
{
    public class CrearDistritoCommandHandler : IRequestHandler<CrearDistritoCommand, ErrorOr<Distrito>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearDistritoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Distrito>> Handle(CrearDistritoCommand command, CancellationToken cancellationToken)
        {
            // Verifica tamaño del código
            if (Validadores.Codigo5(command.Distrito.Codigo))
            {
                return ErroresDistrito.DistritoCodigoInvalido;
            }

            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.Nombre50(command.Distrito.Nombre))
            {
                return ErroresDistrito.DistritoNombreInvalido;
            }

            var canton = await _unitOfWork.CantonesRepository.ObtenerCantonesPorId(command.Distrito.IdCanton);
            if (canton.IsError)
            {
                return canton.Errors;
            }

            var existe = await _unitOfWork.DistritosRepository.ValidarDistrito(command.Distrito.Id, command.Distrito.Codigo, 0);
            if (existe.Value)
            {
                return ErroresDistrito.DistritoDatosDuplicados;
            }

            var result = await _unitOfWork.DistritosRepository.CrearDistrito(command.Distrito);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return result;
        }
    }
}
