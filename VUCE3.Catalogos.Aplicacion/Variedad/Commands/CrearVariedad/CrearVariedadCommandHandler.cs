using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;


namespace VUCE3.Catalogos.Aplicacion.Variedad.Commands.CrearVariedad
{
    public class CrearVariedadCommandHandler : IRequestHandler<CrearVariedadCommand, ErrorOr<Dominio.Entidades.Variedad>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearVariedadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Variedad>> Handle(CrearVariedadCommand request, CancellationToken cancellationToken)
        {
            //Verifica el tamaño de Nombre o si es vacío  
            if (Validadores.Nombre(request.Variedad.Nombre))
            {
                return ErroresVariedad.VariedadNombreInvalido;
            }

            //Verifica el tamaño de Codigo o si es vacío  
            if (Validadores.Codigo17(request.Variedad.Codigo))
            {
                return ErroresVariedad.VariedadCodigoInvalido;
            }

            //Verifica si existe variedad
            var existe = await _unitOfWork.VariedadesRepository.ValidarVariedad(request.Variedad.Id, request.Variedad.Codigo, request.Variedad.Nombre);            
            if (existe.Value)
            {
                return ErroresVariedad.DatosDuplicados;
            }

            var result = await _unitOfWork.VariedadesRepository.CrearVariedad(request.Variedad);

            await _unitOfWork.Save();
            return result;
        }
    }
}
