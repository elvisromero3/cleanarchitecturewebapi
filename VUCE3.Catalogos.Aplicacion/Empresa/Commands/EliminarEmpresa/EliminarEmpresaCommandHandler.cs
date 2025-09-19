using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EliminarEmpresa
{
    public class EliminarEmpresaCommandHandler : IRequestHandler<EliminarEmpresaCommand, ErrorOr<Dominio.Entidades.Empresa>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarEmpresaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Empresa>> Handle(EliminarEmpresaCommand command, CancellationToken cancellationToken)
        {
            var empresa = await _unitOfWork.EmpresasRepository.ObtenerEmpresaPorId(command.Id);

            if (empresa.IsError)
            {
                return empresa.Errors;
            }

            var result = await _unitOfWork.EmpresasRepository.EliminarEmpresa(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return empresa;
        }
    }
}
