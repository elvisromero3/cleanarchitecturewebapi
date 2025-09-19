using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;

namespace VUCE3.Catalogos.Aplicacion.Requisitos.Command.EliminarRequisito
{
    public class EliminarRequisitoCommandHandler : IRequestHandler<EliminarRequisitoCommand, ErrorOr<Requisito>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarRequisitoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Requisito>> Handle(EliminarRequisitoCommand command, CancellationToken cancellationToken)
        {
            var requisito = await _unitOfWork.RequisitosRepository.ObtenerRequisitoPorId(command.Id);
            if (requisito.IsError)
            {
                return requisito.Errors;
            }

            var result = await _unitOfWork.RequisitosRepository.EliminarRequisito(command.Id);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return requisito;
        }
    }
}
