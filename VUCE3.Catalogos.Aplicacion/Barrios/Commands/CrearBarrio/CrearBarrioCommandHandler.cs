using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Barrios.Commands.CrearBarrio
{
    public class CrearBarrioCommandHandler : IRequestHandler<CrearBarrioCommand, ErrorOr<Barrio>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearBarrioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Barrio>> Handle(CrearBarrioCommand command, CancellationToken cancellationToken)
        {
            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.Nombre100(command.Barrio.Nombre))
            {
                return ErroresBarrio.BarrioNombreInvalido;
            }

            //Verifica tamaño de código o si es vacío 
            if (Validadores.Codigo7(command.Barrio.Codigo))
            {
                return ErroresBarrio.BarrioCodigoInvalido;
            }

            var distrito = await _unitOfWork.DistritosRepository.ObtenerDistritoPorId(command.Barrio.IdDistrito);
            if (distrito.IsError)
            {
                return distrito.Errors;
            }

            var existe = await _unitOfWork.BarriosRepository.ValidarBarrio(command.Barrio.Id, command.Barrio.Codigo, 0);
            if (existe.Value)
            {
                return ErroresBarrio.BarrioDatosDuplicados;
            }

            var result = await _unitOfWork.BarriosRepository.CrearBarrio(command.Barrio);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return result;
        }
    }
}
