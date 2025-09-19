using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Commands.EditarEmpresa
{
    public class EditarEmpresaCommandHandler : IRequestHandler<EditarEmpresaCommand, ErrorOr<Tuple<Dominio.Entidades.Empresa, Dominio.Entidades.Empresa>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarEmpresaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Empresa, Dominio.Entidades.Empresa>>> Handle(EditarEmpresaCommand command, CancellationToken cancellationToken)
        {
            //Verifica si IdProfesional es igual a 0  
            if (command.ListaCambios.Contains("IdProfesional") && command.Empresa.IdProfesional == 0)
            {
                return ErroresEmpresas.EmpresasIdProfesionalInvalido;
            }

            var empresa = await _unitOfWork.EmpresasRepository.ObtenerEmpresaPorId(command.IdEmpresa);
            if (empresa.IsError)
            {
                return empresa.Errors;
            }

            var comprobarIdTipoIdentificacion = empresa.Value.IdTipoIdentificacion;
            var comprobarNumeroIdentificacion = empresa.Value.NumeroIdentificacion;
            var comprobarIdProfesional = empresa.Value.IdProfesional;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("IdTipoIdentificacion"))
            {
                comprobarIdTipoIdentificacion = command.Empresa.IdTipoIdentificacion;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("NumeroIdentificacion"))
            {
                comprobarNumeroIdentificacion = command.Empresa.NumeroIdentificacion;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("IdProfesional"))
            {
                comprobarIdProfesional = command.Empresa.IdProfesional;
                comprobarDuplicado = true;
            }

            //se obtiene copia de la empresa antes de aplicar cambios
            Dominio.Entidades.Empresa empresaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Empresa>(
            JsonConvert.SerializeObject(empresa.Value, Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                })
            ) ?? empresa.Value;

            if (command.ListaCambios.Contains("IdTipoIdentificacion"))
            {
                if (command.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                command.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                command.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                command.Empresa.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresEmpresas.TipoIdentificacionNoEncontrado;
                }

                comprobarIdTipoIdentificacion = command.Empresa.IdTipoIdentificacion;
            }

            if (command.ListaCambios.Contains("Nombre") && Validadores.Nombre(command.Empresa.Nombre))
            {
                return ErroresEmpresas.EmpresaNombreExcedeLimite;
            }

            if (command.ListaCambios.Contains("NumeroIdentificacion") || command.ListaCambios.Contains("IdTipoIdentificacion"))
            {
                if (Validadores.NumeroIdentificacion(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresEmpresas.NumeroIdentificacionExcedeLimite;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion))
                {
                    return ErroresEmpresas.TipoIdentificacionFisicaComienza0;
                }
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.EmpresasRepository.ValidarEmpresa(command.IdEmpresa, comprobarIdTipoIdentificacion, comprobarNumeroIdentificacion, comprobarIdProfesional);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresEmpresas.DatosDuplicados;
                }
            }
            

            var result = await _unitOfWork.EmpresasRepository.ActualizarEmpresa(command.Empresa, command.IdEmpresa, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(empresaAntes, empresa.Value);
        }
    }
}
