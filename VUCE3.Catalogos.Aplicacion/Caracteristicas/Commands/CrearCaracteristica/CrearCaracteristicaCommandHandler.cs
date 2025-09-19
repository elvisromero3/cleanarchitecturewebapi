using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.CrearCaracteristica
{
    public class CrearCaracteristicaCommandHandler : IRequestHandler<CrearCaracteristicaCommand, ErrorOr<Caracteristica>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;

        public CrearCaracteristicaCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }
        public async Task<ErrorOr<Caracteristica>> Handle(CrearCaracteristicaCommand command, CancellationToken cancellationToken)
        {
            if (Validadores.institucionDCAoDIPOA(command.Caracteristica.IdInstitucion))
            {
                return ErroresCaracteristica.CaracteristicaInstitucionInvalida;
            }

            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.LongitudMaximaNoNull(command.Caracteristica.Nombre, 150))
            {
                return ErroresCaracteristica.CaracteristicaNombreInvalido;
            }

            var existe = await _unitOfWork.CaracteristicasRepository.ValidarCaracteristica(command.Caracteristica.Id, command.Caracteristica.IdInstitucion, command.Caracteristica.Nombre);

            if (existe.Value)
            {
                return ErroresCaracteristica.CaracteristicaDatosDuplicados;
            }

            var result = await _unitOfWork.CaracteristicasRepository.CrearCaracteristica(command.Caracteristica);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return result;
        }
    }
}
