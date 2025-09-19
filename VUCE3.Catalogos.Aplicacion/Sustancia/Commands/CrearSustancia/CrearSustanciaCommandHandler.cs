using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Sustancia.Commands.CrearSustancia
{
    public class CrearSustanciaCommandHandler : IRequestHandler<CrearSustanciaCommand, ErrorOr<Dominio.Entidades.Sustancia>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearSustanciaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Sustancia>> Handle(CrearSustanciaCommand command, CancellationToken cancellationToken)
        {
            if (Validadores.LongitudMaximaNoNull(command.Sustancia.Nombre, 300))
            {
                return ErroresSustancia.SustanciaNombreInvalido;
            }

            if (Validadores.LongitudMaximaNoNull(command.Sustancia.Cas, 100))
            {
                return ErroresSustancia.SustanciasCasInvalido;
            }

            if (Validadores.LongitudMaximaNoNull(command.Sustancia.ListaCaq, 100))
            {
                return ErroresSustancia.SustanciaListaCaqInvalido;
            }

            var existe = await _unitOfWork.SustanciasRepository.ValidarSustancia(0, command.Sustancia.Nombre, command.Sustancia.Cas, command.Sustancia.ListaCaq);

            if (existe.Value)
            {
                return ErroresSustancia.DatosDuplicados;
            }

            var result = await _unitOfWork.SustanciasRepository.CrearSustancia(command.Sustancia);

            await _unitOfWork.Save();

            return result;
        }
    }
}

