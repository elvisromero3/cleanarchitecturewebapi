using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.CrearAduana
{
    public class CrearAduanaCommandHandler : IRequestHandler<CrearAduanaCommand, ErrorOr<Aduana>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearAduanaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Aduana>> Handle(CrearAduanaCommand command, CancellationToken cancellationToken)
        {
            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.Nombre(command.Aduana.Nombre))
            {
                return ErroresAduana.AduanaNombreInvalido;
            }

            var existe = await _unitOfWork.AduanasRepository.ValidarAduana(command.Aduana);

            if (existe.Value)
            {
                return ErroresAduana.DatosDuplicados;
            }

            var result = await _unitOfWork.AduanasRepository.CrearAduana(command.Aduana);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return result;
        }
    }
}
