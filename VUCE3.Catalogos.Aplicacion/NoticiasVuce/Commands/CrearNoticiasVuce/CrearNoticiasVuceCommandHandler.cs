using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.CrearNoticiasVuce
{
    public class CrearNoticiasVuceCommandHandler : IRequestHandler<CrearNoticiasVuceCommand, ErrorOr<Dominio.Entidades.NoticiasVuce>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearNoticiasVuceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.NoticiasVuce>> Handle(CrearNoticiasVuceCommand request, CancellationToken cancellationToken)
        {
            //Verifica tamaño Titulo   
            if (Validadores.Titulo(request.NoticiasVuce.Titulo))
            {
                return ErroresNoticiasVuce.NoticiasVuceTituloInvalido;
            }

            //Verifica tamaño TituloIngles   
            if (Validadores.Titulo(request.NoticiasVuce.TituloIngles))
            {
                return ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;
            }

            // Verifica si Texto es vacío
            if (string.IsNullOrWhiteSpace(request.NoticiasVuce.Texto))
            {
                return ErroresNoticiasVuce.NoticiasVuceTextoInvalido;
            }

            // Verifica si TextoIngles es vacío
            if (string.IsNullOrWhiteSpace(request.NoticiasVuce.TextoIngles))
            {
                return ErroresNoticiasVuce.NoticiasVuceTextoInglesInvalido;
            }

            if (!string.IsNullOrWhiteSpace(request.NoticiasVuce.Enlace))
            {
                // Verifica tamaño enlace
                if (Validadores.Enlace(request.NoticiasVuce.Enlace))
                {
                    return ErroresNoticiasVuce.NoticiasLinkExcedeLimite;
                }

                // Verifica valido
                if (Validadores.UrlInvalida(request.NoticiasVuce.Enlace))
                {
                    return ErroresNoticiasVuce.NoticiasLinkInValido;
                }
            }

            var existe = await _unitOfWork.NoticiasVuceRepository.ValidarNoticiasVuce(request.NoticiasVuce.Titulo, request.NoticiasVuce.Texto, request.NoticiasVuce.Enlace, request.NoticiasVuce.TituloIngles, request.NoticiasVuce.TextoIngles);            
            if (existe.Value)
            {
                return ErroresNoticiasVuce.DatosDuplicados;
            }

            var result = await _unitOfWork.NoticiasVuceRepository.CrearNoticiasVuce(request.NoticiasVuce);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
