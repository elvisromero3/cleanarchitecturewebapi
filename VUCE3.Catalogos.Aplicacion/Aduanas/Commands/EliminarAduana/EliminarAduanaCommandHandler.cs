using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Aduanas.Commands.EliminaAduana
{
    public class EliminarAduanaCommandHandler : IRequestHandler<EliminarAduanaCommand, ErrorOr<Aduana>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoGestionUsuariosService;

        public EliminarAduanaCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoGestionUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoGestionUsuariosService = accesoGestionUsuariosService;
        }

        public async Task<ErrorOr<Aduana>> Handle(EliminarAduanaCommand command, CancellationToken cancellationToken)
        {
            var aduana = await _unitOfWork.AduanasRepository.ObtenerAduanaPorId(command.Id);

            if (aduana.IsError)
            {
                return aduana.Errors;
            }

            var existeRelacion = await _accesoGestionUsuariosService.ExisteRelacionAduanaInstitucionesAutorizadas(command.Id);
            if (existeRelacion.IsError)
            {
                return ErroresAduana.FalloServicioAccesoGestionUsuario;
            }
            if (existeRelacion.Value)
            {
                return ErroresAduana.ExisteInstitucionesAutorizadas;
            }

            var result = await _unitOfWork.AduanasRepository.EliminarAduana(command.Id);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            
            return aduana;
        }
    }
}


