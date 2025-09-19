using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.CrearCultivo
{
    public class CrearCultivoCommandHandler : IRequestHandler<CrearCultivoCommand, ErrorOr<Dominio.Entidades.Cultivo>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearCultivoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Cultivo>> Handle(CrearCultivoCommand command, CancellationToken cancellationToken)
        {
            //Verifica si Codigo es vacío o tamaño mayor a 25  
            if (Validadores.Codigo25(command.Cultivo.Codigo))
            {
                return ErroresCultivo.CultivoCodigoInvalido;
            }

            //Verifica si NombreCientifico es vacío o tamaño mayor a 100   
            if (Validadores.Nombre(command.Cultivo.NombreCientifico))
            {
                return ErroresCultivo.CultivoNombreCientificoInvalido;
            }

            //Verifica si Nombre es vacío o tamaño mayor a 100 
            if (Validadores.Nombre(command.Cultivo.Nombre))
            {
                return ErroresCultivo.CultivoNombreInvalido;
            }

            //Verifica si IdVariedad es igual a 0  
            if (command.Cultivo.IdVariedad==0)
            {
                return ErroresCultivo.CultivoIdVariedadInvalido;
            }

            //Verifica si existe cultivo
            var existe = await _unitOfWork.CultivosRepository.ValidarCultivo(command.Cultivo.Id,command.Cultivo.Codigo, command.Cultivo.Nombre, command.Cultivo.NombreCientifico);
            if (existe.Value)
            {
                return ErroresCultivo.DatosDuplicados;
            }

            // Obtiene variedad por id
            var variedad = await _unitOfWork.VariedadesRepository.ObtenerVariedadPorId(command.Cultivo.IdVariedad);
            if (variedad.IsError)
            {
                return variedad.Errors;
            }

            command.Cultivo.Variedad = variedad.Value;

            var result = await _unitOfWork.CultivosRepository.CrearCultivo(command.Cultivo);            

            await _unitOfWork.Save();
            return result;
        }
    }
}
