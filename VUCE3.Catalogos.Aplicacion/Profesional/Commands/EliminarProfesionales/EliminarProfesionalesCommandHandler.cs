using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales
{
    public class EliminarProfesionalesCommandHandler : IRequestHandler<EliminarProfesionalesCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarProfesionalesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarProfesionalesCommand request, CancellationToken cancellationToken)
        {
            foreach(var idProfesional in request.IdsProfesionales)
            {
                var resultDelete = await _unitOfWork.ProfesionalesRepository.EliminarProfesional(idProfesional);

                if (resultDelete.IsError)
                {
                    return resultDelete.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
