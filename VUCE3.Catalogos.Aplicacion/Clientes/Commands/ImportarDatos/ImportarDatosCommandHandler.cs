using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos.DTO;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Constantes;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Clientes.Commands.ImportarDatos
{
    public class ImportarDatosCommandHandler : IRequestHandler<ImportarDatosCommand, ErrorOr<Created>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImportarDatosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Created>> Handle(ImportarDatosCommand request, CancellationToken cancellationToken)
        {
            if (ExistenDuplicados(request.Datos))
            {
                return ErroresCliente.DatosDuplicadosArchivo;
            }


            if (request.Modo == ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
            {
                var datosExistentes = await _unitOfWork.ClientesRepository.ObtenerClientes();

                if (datosExistentes.IsError)
                {
                    return datosExistentes.Errors;
                }

                foreach (var datoExistente in datosExistentes.Value)
                {
                    var resultadoDelete = await _unitOfWork.ClientesRepository.EliminarCliente(datoExistente.Id);

                    if (resultadoDelete.IsError)
                    {
                        return resultadoDelete.Errors;
                    }
                }
            }

            foreach (var datoInsertar in request.Datos)
            {
                var cliente = new Cliente()
                {
                    CodigoCliente = datoInsertar.CodigoCliente,
                    IdTipoIdentificacion = datoInsertar.TipoIdentificacion,
                    FechaVencimiento = datoInsertar.FechaVencimiento,
                    NombreCliente = datoInsertar.NombreCliente,
                    NumeroIdentificacion = datoInsertar.NumeroIdentificacion
                };

                if (datoInsertar.CodigoCliente.Length > 35)
                {
                    return ErroresCliente.ClienteTamanoCodigoCliente;
                }

                //se valida tipo identificación
                if (datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.FISICA &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.DIMEX &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.JURIDICA &&
                    datoInsertar.TipoIdentificacion != ConstantesIdTipoIdentificacion.PASAPORTE)
                {
                    return ErroresCliente.TipoIdentificacionNoEncontrado;
                }

                //se valida tipo identificación y tamaño
                if (Validadores.NumeroIdentificacion(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresCliente.TamanoNroIdentificacion;
                }

                //se valida identificación física comienza con 0
                if (Validadores.TipoIdentificacionFisicaComienza0(datoInsertar.TipoIdentificacion, datoInsertar.NumeroIdentificacion))
                {
                    return ErroresCliente.TipoIdentificacionFisicaComienza0;
                }

                //se valida tipo identificación
                if (datoInsertar.NombreCliente.Length > 100)
                {
                    return ErroresCliente.ClienteTamanoNombreCliente;
                }

                if (datoInsertar.FechaVencimiento.Date <= DateTime.Today)
                {
                    return ErroresCliente.FechaVencimientoMenorHoy;
                }

                //Valida si existe cliente 
                if (request.Modo != ConstantesGenerales.IMPORTARDATOS_MODO_REEMPLAZAR)
                {
                    var existe = await _unitOfWork.ClientesRepository.ValidarCliente(cliente.Id, cliente.CodigoCliente, cliente.NombreCliente, cliente.IdTipoIdentificacion, cliente.NumeroIdentificacion, cliente.FechaVencimiento);
                    if (existe.Value)
                    {
                        return ErroresCliente.ClienteExiste;
                    }
                }

                var resultInsertar = await _unitOfWork.ClientesRepository.CrearCliente(cliente);

                if (resultInsertar.IsError)
                {
                    return resultInsertar.Errors;
                }
            }

            await _unitOfWork.Save();

            return Result.Created;
        }
        private static bool ExistenDuplicados(IEnumerable<ImportarClienteCommandDto> datos)
        {
            HashSet<string> datosComprobados = new HashSet<string>();
            foreach (var cliente in datos)
            {
                if (!datosComprobados.Add(cliente.CodigoCliente + "_" + Validadores.NormalizarString(cliente.NombreCliente) + "_" + cliente.TipoIdentificacion
                    + "_" + cliente.NumeroIdentificacion + "_" + cliente.FechaVencimiento))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
