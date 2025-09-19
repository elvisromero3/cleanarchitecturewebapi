using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.Categoria.Commands.EditarCategoria
{
    public class EditarCategoriaCommandHandler : IRequestHandler<EditarCategoriaCommand, ErrorOr<Tuple<Dominio.Entidades.Categoria, Dominio.Entidades.Categoria>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarCategoriaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.Categoria, Dominio.Entidades.Categoria>>> Handle(EditarCategoriaCommand command, CancellationToken cancellationToken)
        {
            // Obtener Categoria por Id
            var Categoria = await _unitOfWork.CategoriaRepository.ObtenerCategoriaPorId(command.IdCategoria);
            if (Categoria.IsError)
            {
                return Categoria.Errors;
            }

            var comprobarNombre = Categoria.Value.Nombre;
            var comprobarIdInstitucion = Categoria.Value.IdInstitucion;
            var comprobarDuplicado = false;

            //Verifica el tamaño de Nombre o si es vacío  
            if (command.ListaCambios.Contains("Nombre"))
            {
                comprobarNombre = command.Categoria.Nombre;
                comprobarDuplicado = true;

                if (Validadores.LongitudMaximaNoNull(command.Categoria.Nombre, 100))
                {
                    return ErroresCategoria.NombreInvalido;
                }
            }

            if (command.ListaCambios.Contains("IdInstitucion"))
            {
                comprobarIdInstitucion = command.Categoria.IdInstitucion;
                comprobarDuplicado = true;

                if (Validadores.institucionDCAoDIPOA(command.Categoria.IdInstitucion))
                {
                    return ErroresCategoria.CategoriaInstitucionInvalida;
                }
            }

            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.CategoriaRepository.ValidarCategoria(command.IdCategoria, comprobarNombre, comprobarIdInstitucion);
                if (existe.IsError)
                {
                    return existe.Errors;
                }

                if (existe.Value)
                {
                    return ErroresCategoria.DatosDuplicados;
                }
            }

            //se obtiene copia de la Categoria antes de aplicar cambios
            Dominio.Entidades.Categoria CategoriaAntes = JsonConvert.DeserializeObject<Dominio.Entidades.Categoria>(
                JsonConvert.SerializeObject(Categoria.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? Categoria.Value;

            var result = await _unitOfWork.CategoriaRepository.ActualizarCategoria(command.IdCategoria, command.ListaCambios, command.Categoria);
            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(CategoriaAntes, Categoria.Value);
        }
    }
}
