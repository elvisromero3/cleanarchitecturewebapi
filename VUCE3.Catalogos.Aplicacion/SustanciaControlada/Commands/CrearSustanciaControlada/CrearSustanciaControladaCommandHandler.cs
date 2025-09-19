using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.SustanciaControlada.Commands.CrearSustanciaControlada
{
    public class CrearSustanciaControladaCommandHandler : IRequestHandler<CrearSustanciaControladaCommand, ErrorOr<Dominio.Entidades.SustanciaControlada>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearSustanciaControladaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.SustanciaControlada>> Handle(CrearSustanciaControladaCommand request, CancellationToken cancellationToken)
        {
            //se valida ClasificacionArancelaria
            if (Validadores.LongitudMaximaNoNull(request.SustanciaControlada.ClasificacionArancelaria, 100))
            {
                return ErroresSustanciaControlada.SustanciaControladaClasificacionArancelariaTamano;
            }
            //se valida ClasificacionAshrae
            if (Validadores.LongitudMaximaNoNull(request.SustanciaControlada.ClasificacionAshrae, 100))
            {
                return ErroresSustanciaControlada.SustanciaControladaClasificacionAshraeTamano;
            }
            //se valida PotencialCalentamientoGlobal
            if (Validadores.LongitudMaximaNoNull(request.SustanciaControlada.PotencialCalentamientoGlobal, 50))
            {
                return ErroresSustanciaControlada.SustanciaControladaPotencialCalentamientoGlobalTamano;
            }
            //se valida TipoGas
            if (Validadores.LongitudMaximaNoNull(request.SustanciaControlada.TipoGas, 100))
            {
                return ErroresSustanciaControlada.SustanciaControladaTipoGasTamano;
            }

            var familia = await _unitOfWork.SustanciaControladaRepository.ObtenerFamiliaSustanciaControlada(request.SustanciaControlada.IdFamilia);
            if (familia.IsError)
            {
                return familia.Errors;
            }

            request.SustanciaControlada.Familia = familia.Value;

            var existe = await _unitOfWork.SustanciaControladaRepository.ValidarSustanciasControladas(0, request.SustanciaControlada);
            if (existe.Value)
            {
                return ErroresSustanciaControlada.DatosDuplicados;
            }

            var result = await _unitOfWork.SustanciaControladaRepository.CrearSustanciaControlada(request.SustanciaControlada);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
