using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Profesional.Commands.EliminarProfesionales;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresas
{
    public class EliminarEmpresasCommandHandler : IRequestHandler<EliminarEmpresasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarEmpresasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarEmpresasCommand request, CancellationToken cancellationToken)
        {
            foreach (var idEmpresa in request.IdsEmpresas)
            {
                var resultDelete = await _unitOfWork.EmpresasRepository.EliminarEmpresa(idEmpresa);

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
