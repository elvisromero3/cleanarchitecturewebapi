using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesional
{
    public class EliminarProfesionalCommandHandler : IRequestHandler<EliminarProfesionalCommand, ErrorOr<Dominio.Entidades.Profesional>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarProfesionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Profesional>> Handle(EliminarProfesionalCommand command, CancellationToken cancellationToken)
        {
            var profesional = await _unitOfWork.ProfesionalesRepository.ObtenerProfesionalPorId(command.Id);

            if (profesional.IsError)
            {
                return profesional.Errors;
            }

            var result = await _unitOfWork.ProfesionalesRepository.EliminarProfesional(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return profesional;
        }
    }
}
