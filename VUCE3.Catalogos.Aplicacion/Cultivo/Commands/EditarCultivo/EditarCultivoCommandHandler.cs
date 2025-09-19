using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Cultivo.Commands.EditarCultivo
{
    public class EditarCultivoCommandHandler : IRequestHandler<EditarCultivoCommand, ErrorOr<Tuple<Dominio.Entidades.Cultivo, Dominio.Entidades.Cultivo>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarCultivoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ErrorOr<Tuple<Dominio.Entidades.Cultivo, Dominio.Entidades.Cultivo>>> Handle(EditarCultivoCommand command, CancellationToken cancellationToken)
        {
            // Obtiene cultivo por id
            var cultivo = await _unitOfWork.CultivosRepository.ObtenerCultivoPorId(command.IdCultivo);
            if (cultivo.IsError)
            {
                return cultivo.Errors;
            }

            //Verifica si Codigo es vacío o tamaño mayor a 25              
            if (command.ListaCambios.Contains("Codigo") && Validadores.Codigo25(command.Cultivo.Codigo))
            {
                return ErroresCultivo.CultivoCodigoInvalido;
            }

            //Verifica si NombreCientifico es vacío o tamaño mayor a 100   
            if (command.ListaCambios.Contains("NombreCientifico") && Validadores.Nombre(command.Cultivo.NombreCientifico))
            {
                return ErroresCultivo.CultivoNombreCientificoInvalido;
            }

            //Verifica si Nombre es vacío o tamaño mayor a 100 
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre(command.Cultivo.Nombre))
            {
                return ErroresCultivo.CultivoNombreInvalido;
            }

            //Verifica si IdVariedad es igual a 0  
            if (command.ListaCambios.Contains("IdVariedad") && command.Cultivo.IdVariedad == 0)
            {
                return ErroresCultivo.CultivoIdVariedadInvalido;
            }

            var comprobarCodigo = cultivo.Value.Codigo;
            var comprobarNombre = cultivo.Value.Nombre;
            var comprobarNombreCientifico = cultivo.Value.NombreCientifico;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Codigo"))
            {
                comprobarCodigo = command.Cultivo.Codigo;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Cultivo.Nombre;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("NombreCientifico"))
            {
                comprobarNombreCientifico = command.Cultivo.NombreCientifico;
                comprobarDuplicado = true;
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.CultivosRepository.ValidarCultivo(command.IdCultivo, comprobarCodigo, comprobarNombre, comprobarNombreCientifico);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCultivo.DatosDuplicados;
                }
            }

            if (command.ListaCambios.Contains("IdVariedad"))
            {
                var variedad = await _unitOfWork.VariedadesRepository.ObtenerVariedadPorId(command.Cultivo.IdVariedad);

                if (variedad.IsError)
                {
                    return variedad.Errors;
                }
            }

            //se obtiene copia de cultivo antes de aplicar cambios
            Dominio.Entidades.Cultivo cultivoAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Cultivo>(
                JsonConvert.SerializeObject(cultivo.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? cultivo.Value;

            var cultivoEditado = await _unitOfWork.CultivosRepository.EditarCultivo(command.Cultivo, command.IdCultivo, command.ListaCambios);

            if (cultivoEditado.IsError)
            {
                return cultivoEditado.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(cultivoAntes, cultivo.Value);
        }
    }
}
