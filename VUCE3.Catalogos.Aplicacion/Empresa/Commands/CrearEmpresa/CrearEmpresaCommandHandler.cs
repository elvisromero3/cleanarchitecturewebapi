using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.CrearEmpresa
{
    public class CrearEmpresaCommandHandler : IRequestHandler<CrearEmpresaCommand, ErrorOr<Dominio.Entidades.Empresa>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearEmpresaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ErrorOr<Dominio.Entidades.Empresa>> Handle(CrearEmpresaCommand request, CancellationToken cancellationToken)
        {            
            //Verifica si IdProfesional es igual a 0  
            if (request.Empresa.IdProfesional == 0)
            {
                return ErroresEmpresas.EmpresasIdProfesionalInvalido;
            }

            //Verifica si Nombre es vacío  
            if ( Validadores.Nombre(request.Empresa.Nombre))
            {
                return ErroresEmpresas.EmpresaNombreExcedeLimite;
            }           

            var existe = await _unitOfWork.EmpresasRepository.ValidarEmpresa(request.Empresa.Id, request.Empresa.IdTipoIdentificacion,request.Empresa.NumeroIdentificacion,request.Empresa.IdProfesional);
            if (existe.Value)
            {
                return ErroresEmpresas.DatosDuplicados;
            }

            if (request.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                request.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                request.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                request.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
            {
                return ErroresEmpresas.TipoIdentificacionNoEncontrado;
            }

            if (Validadores.NumeroIdentificacion(request.Empresa.IdTipoIdentificacion, request.Empresa.NumeroIdentificacion))
            {
                return ErroresEmpresas.NumeroIdentificacionExcedeLimite;
            }

            //se valida identificación física comienza con 0
            if (Validadores.TipoIdentificacionFisicaComienza0(request.Empresa.IdTipoIdentificacion, request.Empresa.NumeroIdentificacion))
            {
                return ErroresEmpresas.TipoIdentificacionFisicaComienza0;
            }

            var profesional = await _unitOfWork.ProfesionalesRepository.ObtenerProfesionalPorId(request.Empresa.IdProfesional);
            if (profesional.IsError)
            {
                return profesional.Errors;
            }
            request.Empresa.Profesional = profesional.Value;
            var result = await _unitOfWork.EmpresasRepository.CrearEmpresa(request.Empresa);

            if (result.IsError)
            {
                return existe.Errors;
            }

            await _unitOfWork.Save();
            return result;            
        }
    }
}
