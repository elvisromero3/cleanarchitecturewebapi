using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sectores.Commands.EditarSector
{
    public class EditarSectorCommandHandler : IRequestHandler<EditarSectorCommand, ErrorOr<Tuple<Sector, Sector>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarSectorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Sector, Sector>>> Handle(EditarSectorCommand command, CancellationToken cancellationToken)
        {
            var sector = await _unitOfWork.SectoresRepository.ObtenerSectorPorId(command.IdSector);
            if (sector.IsError)
            {
                return sector.Errors;
            }
            var comprobarCodigo = sector.Value.Codigo;
            var comprobarNombre = sector.Value.Nombre;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Sector.Codigo;
                comprobarDuplicado = true;

                if(Validadores.LongitudMaximaNoNull(command.Sector.Codigo, 10))
                {
                    return ErroresSector.SectorCodigoInvalido;
                }
            }

            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Sector.Nombre;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Sector.Nombre, 50))
                {
                    return ErroresSector.SectorNombreInvalido;
                }
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.SectoresRepository.ValidarSector(command.IdSector, comprobarNombre, comprobarCodigo);

                if (existe.Value)
                {
                    return ErroresSector.SectorDatosDuplicados;
                }
            }

            //se obtiene copia del sector antes de aplicar cambios
            Sector sectorAntes = JsonConvert.DeserializeObject<Sector>(
                JsonConvert.SerializeObject(sector.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? sector.Value;

            var result = await _unitOfWork.SectoresRepository.ActualizarSector(command.Sector, command.IdSector, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(sectorAntes, sector.Value);
        }

    }
}
