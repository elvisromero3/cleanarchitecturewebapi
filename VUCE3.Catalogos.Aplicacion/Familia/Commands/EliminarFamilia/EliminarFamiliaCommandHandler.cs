using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Familia.Commands.EliminarFamilia
{
    public class EliminarFamiliaCommandHandler : IRequestHandler<EliminarFamiliaCommand, ErrorOr<Dominio.Entidades.Familia>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarFamiliaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Familia>> Handle(EliminarFamiliaCommand command, CancellationToken cancellationToken)
        {
            var familia = await _unitOfWork.FamiliaRepository.ObtenerFamiliaPorId(command.Id);
            if (familia.IsError)
            {
                return familia.Errors;
            }

            var sustancias = await _unitOfWork.SustanciaControladaRepository.ObtenerSustanciaControladas();
            var sustanciasPorFamilia = sustancias.Value.Count(x => x.IdFamilia == command.Id);
            if (sustanciasPorFamilia > 0)
            {
                return ErroresFamilia.SustanciasControladasRelacionadas;
            }

            var result = await _unitOfWork.FamiliaRepository.EliminarFamilia(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return familia;
        }
    }
}
