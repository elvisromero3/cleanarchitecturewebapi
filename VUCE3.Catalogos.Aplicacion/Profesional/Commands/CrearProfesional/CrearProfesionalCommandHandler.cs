using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Profesional.Commands.CrearProfesional
{
    public class CrearProfesionalCommandHandler : IRequestHandler<CrearProfesionalCommand, ErrorOr<Dominio.Entidades.Profesional>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearProfesionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Dominio.Entidades.Profesional>> Handle(CrearProfesionalCommand request, CancellationToken cancellationToken)
        {
            // se valida el tamaño de email
            if (request.Profesional.IdInstitucion == 0)
            {
                return ErroresProfesionales.IdInstitucionInvalido;
            }

            // se valida el tamaño de email
            if (request.Profesional.Email.Length > 100)
            {
                return ErroresProfesionales.TamanoEmail;
            }

            //se valida formato de email
            if (Validadores.EmailInvalido(request.Profesional.Email))
            {
                return ErroresProfesionales.EmailInvalido;
            }

            //se valida tipo identificación
            if (request.Profesional.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                request.Profesional.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX)
            {
                return ErroresProfesionales.TipoIdentificacionIncorrecta;
            }

            //se valida tipo identificación y tamaño
            if (Validadores.NumeroIdentificacion(request.Profesional.IdTipoIdentificacion, request.Profesional.NumeroIdentificacion))
            {
                return ErroresProfesionales.TamanoNroIdentificacion;
            }

            //se valida identificación física comienza con 0
            if (Validadores.TipoIdentificacionFisicaComienza0(request.Profesional.IdTipoIdentificacion, request.Profesional.NumeroIdentificacion))
            {
                return ErroresProfesionales.TipoIdentificacionFisicaComienza0;
            }

            //se valida tamaño de nombre
            if (Validadores.Nombre(request.Profesional.Nombre))
            {
                return ErroresProfesionales.TamanoNombre;
            }

            //se valida tamaño profesión
            if (Validadores.LongitudMaximaNoNull(request.Profesional.Profesion, 100))
            {
                return ErroresProfesionales.TamanoProfesion;
            }

            //se valida tamaño código regente
            if (Validadores.Codigo20(request.Profesional.CodigoRegente))
            {
                return ErroresProfesionales.TamanoCodigoRegente;
            }

            //Validar existe profesional
            var existe = await _unitOfWork.ProfesionalesRepository.ValidarProfesional(request.Profesional.Id, request.Profesional.NumeroIdentificacion, request.Profesional.IdInstitucion);
            if (existe.Value)
            {
                return ErroresProfesionales.DatosDuplicados;
            }
            
            var result = await _unitOfWork.ProfesionalesRepository.CrearProfesional(request.Profesional);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
