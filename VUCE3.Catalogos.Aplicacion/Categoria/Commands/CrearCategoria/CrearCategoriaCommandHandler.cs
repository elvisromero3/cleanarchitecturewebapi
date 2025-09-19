using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.CrearCategoria
{
    public class CrearCategoriaCommandHandler : IRequestHandler<CrearCategoriaCommand, ErrorOr<Dominio.Entidades.Categoria>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CrearCategoriaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Dominio.Entidades.Categoria>> Handle(CrearCategoriaCommand request, CancellationToken cancellationToken)
        {
            //Verifica el tamaño de Nombre o si es vacío  
            if (Validadores.Nombre(request.Categoria.Nombre))
            {
                return ErroresCategoria.NombreInvalido;
            }
            //Verifica tamaño de Nombre o si es vacío 
            if (Validadores.LongitudMaximaNoNull(request.Categoria.Nombre, 100))
            {
                return ErroresCategoria.NombreInvalido;
            }

            if (Validadores.institucionDCAoDIPOA(request.Categoria.IdInstitucion))
            {
                return ErroresCategoria.CategoriaInstitucionInvalida;
            }

            var existe = await _unitOfWork.CategoriaRepository.ValidarCategoria(request.Categoria.Id, request.Categoria.Nombre, request.Categoria.IdInstitucion);
            if (existe.Value)
            {
                return ErroresCategoria.DatosDuplicados;
            }


            var result = await _unitOfWork.CategoriaRepository.CrearCategoria(request.Categoria);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();
            return result;
        }
    }
}
