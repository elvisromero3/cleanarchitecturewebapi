using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilias
{
    public class EliminarFamiliasCommandHandler :IRequestHandler<EliminarFamiliasCommand, ErrorOr<Deleted>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EliminarFamiliasCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Deleted>> Handle(EliminarFamiliasCommand request, CancellationToken cancellationToken)
        {            
            var sustanciasControladas = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladas();
            
            foreach (var idFamilia in request.IdsFamilias)
            {
                var sustanciasControladasFamilia = sustanciasControladas.Value.Count(p => p.IdFamilia == idFamilia);
                if (sustanciasControladasFamilia > 0)
                {
                    return ErroresFamilia.SustanciasControladasRelacionadas;
                }
            }

            foreach (var idFamilia in request.IdsFamilias)
            {
                var result = await _unitOfWork.FamiliaRepository.EliminarFamilia(idFamilia);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Deleted;
        }
    }
}
