using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;
namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.CrearCliente
{
    public class CrearClienteCommandHandler : IRequestHandler<CrearClienteCommand, ErrorOr<Cliente>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CrearClienteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Cliente>> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
        {
            //Verifica si Nombre es vacío  
            if (Validadores.Nombre(request.Cliente.NombreCliente))
            {
                return ErroresCliente.ClienteTamanoNombreCliente;
            }

            //Verifica si CodigoCliente es vacío  
            if (Validadores.Codigo35(request.Cliente.CodigoCliente))
            {
                return ErroresCliente.ClienteTamanoCodigoCliente;
            }

            //se valida tipo identificación
            if (request.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                request.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                request.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                request.Cliente.IdTipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
            {
                return ErroresCliente.TipoIdentificacionNoEncontrado;
            }

            //se valida tipo identificación y tamaño
            if (Validadores.NumeroIdentificacion(request.Cliente.IdTipoIdentificacion, request.Cliente.NumeroIdentificacion))
            {
                return ErroresCliente.TamanoNroIdentificacion;
            }

            //se valida identificación física comienza con 0
            if (Validadores.TipoIdentificacionFisicaComienza0(request.Cliente.IdTipoIdentificacion, request.Cliente.NumeroIdentificacion))
            {
                return ErroresCliente.TipoIdentificacionFisicaComienza0;
            }

            if (request.Cliente.FechaVencimiento.Date <= DateTime.Today)
            {
                return ErroresCliente.FechaVencimientoMenorHoy;
            }

            //Valida si existe cliente
            var existe = await _unitOfWork.ClientesRepository.ValidarCliente(request.Cliente.Id, request.Cliente.CodigoCliente, request.Cliente.NombreCliente, request.Cliente.IdTipoIdentificacion, request.Cliente.NumeroIdentificacion, request.Cliente.FechaVencimiento);
            if (existe.Value)
            {
                return ErroresCliente.ClienteExiste;
            }

            var result = await _unitOfWork.ClientesRepository.CrearCliente(request.Cliente);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
