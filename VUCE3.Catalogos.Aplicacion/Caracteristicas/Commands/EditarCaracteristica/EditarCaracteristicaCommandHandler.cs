using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Servicios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Caracteristicas.Commands.EditarCaracteristica
{
    public class EditarCaracteristicaCommandHandler : IRequestHandler<EditarCaracteristicaCommand, ErrorOr<Tuple<Caracteristica, Caracteristica>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccesoGestionUsuariosService _accesoUsuariosService;

        public EditarCaracteristicaCommandHandler(IUnitOfWork unitOfWork, IAccesoGestionUsuariosService accesoUsuariosService)
        {
            _unitOfWork = unitOfWork;
            _accesoUsuariosService = accesoUsuariosService;
        }

        public async Task<ErrorOr<Tuple<Caracteristica, Caracteristica>>> Handle(EditarCaracteristicaCommand command, CancellationToken cancellationToken)
        {
            var caracteristica = await _unitOfWork.CaracteristicasRepository.ObtenerCaracteristicaPorId(command.IdCaracteristica);
            if (caracteristica.IsError)
            {
                return caracteristica.Errors;
            }

            var comprobarIdInstitucion = caracteristica.Value.IdInstitucion;
            var comprobarNombre = caracteristica.Value.Nombre;
            var comprobarDuplicado = false;

            //Verifica el tamaño de Nombre o si es vacío  
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Caracteristica.Nombre;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Caracteristica.Nombre, 150))
                {
                    return ErroresCaracteristica.CaracteristicaNombreInvalido;
                }
            }

            //Verifica institución
            if (command.ListaCambios.Contains("IdInstitucion"))
            {
                comprobarIdInstitucion = command.Caracteristica.IdInstitucion;
                comprobarDuplicado = true;

                if (Validadores.institucionDCAoDIPOA(command.Caracteristica.IdInstitucion))
                {
                    return ErroresCaracteristica.CaracteristicaInstitucionInvalida;
                }
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.CaracteristicasRepository.ValidarCaracteristica(command.IdCaracteristica, comprobarIdInstitucion, comprobarNombre);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCaracteristica.CaracteristicaDatosDuplicados;
                }
            }
            
            //se obtiene copia de característica antes de aplicar cambios
            Caracteristica caracteristicaAntes = JsonConvert.DeserializeObject<Caracteristica>(
                JsonConvert.SerializeObject(caracteristica.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? caracteristica.Value;


            var result = await _unitOfWork.CaracteristicasRepository.ActualizarCaracteristica(command.Caracteristica, command.IdCaracteristica, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(caracteristicaAntes, caracteristica.Value);
        }
    }
}
