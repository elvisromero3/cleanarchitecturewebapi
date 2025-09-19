using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.EditarProfesional
{
    public class EditarProfesionalCommandHandler : IRequestHandler<EditarProfesionalCommand, ErrorOr<Tuple<Dominio.Entidades.Profesional, Dominio.Entidades.Profesional>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarProfesionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Profesional, Dominio.Entidades.Profesional>>> Handle(EditarProfesionalCommand command, CancellationToken cancellationToken)
        {
            //Se verifica IdInstitucion 
            if (command.ListaCambios.Contains("IdInstitucion") && command.Profesional.IdInstitucion == 0)
            {
                return ErroresProfesionales.IdInstitucionInvalido;
            }

            // Se verifica la existencia de profesional
            var profesional = await _unitOfWork.ProfesionalesRepository.ObtenerProfesionalPorId(command.IdProfesional);
            if (profesional.IsError)
            {
                return profesional.Errors;
            }

            var comprobarIdTipoIdentificacion = profesional.Value.IdTipoIdentificacion;
            var comprobarNumeroIdentificacion = profesional.Value.NumeroIdentificacion;
            var comprobarIdInstitucion = profesional.Value.IdInstitucion;
            var comprobarDuplicado = false;

            //se obtiene copia de profesional antes de aplicar cambios
            Dominio.Entidades.Profesional profesionalAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Profesional>(
                JsonConvert.SerializeObject(profesional.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? profesional.Value;

            if (command.ListaCambios.Contains("IdInstitucion"))
            {
                comprobarIdInstitucion = command.Profesional.IdInstitucion;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("IdTipoIdentificacion"))
            {
                //se valida tipo identificación
                if (command.Profesional.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                command.Profesional.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX)
                {
                    return ErroresProfesionales.TipoIdentificacionIncorrecta;
                }

                comprobarIdTipoIdentificacion = command.Profesional.IdTipoIdentificacion;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("NumeroIdentificacion"))
            {
                comprobarNumeroIdentificacion = command.Profesional.NumeroIdentificacion;
                comprobarDuplicado = true;
            }

            if (command.ListaCambios.Contains("NumeroIdentificacion") || command.ListaCambios.Contains("IdTipoIdentificacion"))
            {
                if (Validadores.NumeroIdentificacion(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresProfesionales.TamanoNroIdentificacion;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresProfesionales.TipoIdentificacionFisicaComienza0;
                }
            }

            if (command.ListaCambios.Contains("Email"))
            {
                //se valida tamaño máximo de email
                if (command.Profesional.Email.Length > 100)
                {
                    return ErroresProfesionales.TamanoEmail;
                }

                if (Validadores.EmailInvalido(command.Profesional.Email))
                {
                    return ErroresProfesionales.EmailInvalido;
                }
            }

            //se valida tamaño máximo de nombre
            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre(command.Profesional.Nombre))
            {
                return ErroresProfesionales.TamanoNombre;
            }

            //se valida tamaño máximo de profesion
            if (command.ListaCambios.Contains("Profesion") && Validadores.LongitudMaximaNoNull(command.Profesional.Profesion, 100))
            {
                return ErroresProfesionales.TamanoProfesion;
            }

            //se valida tamaño máximo de código regente
            if (command.ListaCambios.Contains("CodigoRegente") && Validadores.Codigo20(command.Profesional.CodigoRegente))
            {
                return ErroresProfesionales.TamanoCodigoRegente;
            }

            //Se verifica si existe profesional
            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.ProfesionalesRepository.ValidarProfesional(command.IdProfesional, comprobarNumeroIdentificacion, comprobarIdInstitucion);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresProfesionales.DatosDuplicados;
                }
            }

            var result = await _unitOfWork.ProfesionalesRepository.ActualizarProfesional(command.Profesional, command.IdProfesional, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return Tuple.Create(profesionalAntes, profesional.Value);
        }
    }
}
