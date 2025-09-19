using ErrorOr;
using MediatR;
using Newtonsoft.Json;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Aplicacion.NoticiasVuce.Commands.EditarNoticiasVuce
{
    public class EditarNoticiasVuceCommandHandler : IRequestHandler<EditarNoticiasVuceCommand, ErrorOr<Tuple<Dominio.Entidades.NoticiasVuce, Dominio.Entidades.NoticiasVuce>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditarNoticiasVuceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Tuple<Dominio.Entidades.NoticiasVuce, Dominio.Entidades.NoticiasVuce>>> Handle(EditarNoticiasVuceCommand command, CancellationToken cancellationToken)
        {
            if(command.ListaCambios.Contains("Titulo") && Validadores.Titulo(command.NoticiasVuce.Titulo))
            {
                return ErroresNoticiasVuce.NoticiasVuceTituloInvalido;
            }

            if (command.ListaCambios.Contains("TituloIngles") && Validadores.Titulo(command.NoticiasVuce.TituloIngles))
            {
                return ErroresNoticiasVuce.NoticiasVuceTituloInglesInvalido;
            }

            // Verifica si Texto es vacío
            if (command.ListaCambios.Contains("Texto") && string.IsNullOrWhiteSpace(command.NoticiasVuce.Texto))
            {
                return ErroresNoticiasVuce.NoticiasVuceTextoInvalido;
            }

            // Verifica si TextoIngles es vacío
            if (command.ListaCambios.Contains("TextoIngles") && string.IsNullOrWhiteSpace(command.NoticiasVuce.TextoIngles))
            {
                return ErroresNoticiasVuce.NoticiasVuceTextoInglesInvalido;
            }

            if (command.ListaCambios.Contains("Enlace") && command.NoticiasVuce.Enlace is not null)
            {
                if (Validadores.Enlace(command.NoticiasVuce.Enlace)) {
                    return ErroresNoticiasVuce.NoticiasLinkExcedeLimite;
                }

                if (Validadores.UrlInvalida(command.NoticiasVuce.Enlace))
                {
                    return ErroresNoticiasVuce.NoticiasLinkInValido;
                }
            }

            var noticiaVuce = await _unitOfWork.NoticiasVuceRepository.ObtenerNoticiasVucePorId(command.IdNoticiasVuce);

            if (noticiaVuce.IsError)
            {
                return noticiaVuce.Errors;
            }

            var comprobarTitulo = noticiaVuce.Value.Titulo;
            var comprobarTexto = noticiaVuce.Value.Texto;
            var comprobarEnlace = noticiaVuce.Value.Enlace;
            var comprobarTituloIngles = noticiaVuce.Value.TituloIngles;
            var comprobarTextoIngles = noticiaVuce.Value.TextoIngles;
            var comprobarDuplicado = false;

            if (command.ListaCambios.Contains("Titulo"))
            {
                comprobarTitulo = command.NoticiasVuce.Titulo;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("Texto"))
            {
                comprobarTexto = command.NoticiasVuce.Texto;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("Enlace"))
            {
                comprobarEnlace = command.NoticiasVuce.Enlace;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("TituloIngles"))
            {
                comprobarTituloIngles = command.NoticiasVuce.TituloIngles;
                comprobarDuplicado = true;
            }
            if (command.ListaCambios.Contains("TextoIngles"))
            {
                comprobarTextoIngles = command.NoticiasVuce.TextoIngles;
                comprobarDuplicado = true;
            }
            if (comprobarDuplicado)
            {
                var existe = await _unitOfWork.NoticiasVuceRepository.ValidarNoticiasVuce(comprobarTitulo,comprobarTexto,comprobarEnlace,comprobarTituloIngles,comprobarTextoIngles);
                if (existe.IsError)
                {
                    return existe.Errors;
                }
                if (existe.Value)
                {
                    return ErroresNoticiasVuce.DatosDuplicados;
                }
            }
           
            //se obtiene copia de la noticia VUCE antes de aplicar cambios
            Dominio.Entidades.NoticiasVuce noticiaVuceAntes = JsonConvert.DeserializeObject<Dominio.Entidades.NoticiasVuce>(
                JsonConvert.SerializeObject(noticiaVuce.Value, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                ) ?? noticiaVuce.Value;

            var result = await _unitOfWork.NoticiasVuceRepository.ActualizarNoticiasVuce(command.NoticiasVuce, command.IdNoticiasVuce, command.ListaCambios);

            if (result.IsError)
            {
                return result.Errors;
            }

            await _unitOfWork.Save();

            return Tuple.Create(noticiaVuceAntes, noticiaVuce.Value);
        }
    }
}
