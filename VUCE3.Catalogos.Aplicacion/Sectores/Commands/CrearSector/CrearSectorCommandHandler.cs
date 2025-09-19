using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.CrearSector
{
    public class CrearSectorCommandHandler : IRequestHandler<CrearSectorCommand, ErrorOr<Sector>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearSectorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Sector>> Handle(CrearSectorCommand request, CancellationToken cancellationToken)
        { 
            //Verifica el tamaño de Nombre o si es vacío  
            if (Validadores.LongitudMaximaNoNull(request.Sector.Nombre,50))
            {
                return ErroresSector.SectorNombreInvalido;
            }
            //Verifica el tamaño de codigo o se es vacio
            if (Validadores.LongitudMaximaNoNull(request.Sector.Codigo,10))
            {
                return ErroresSector.SectorCodigoInvalido;
            }

            var existe = await _unitOfWork.SectoresRepository.ValidarSector(request.Sector.Id, request.Sector.Nombre, request.Sector.Codigo);
            if (existe.Value)
            {
                return ErroresSector.SectorDatosDuplicados;
            }

            var result = await _unitOfWork.SectoresRepository.CrearSector(request.Sector);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }

    }
}
