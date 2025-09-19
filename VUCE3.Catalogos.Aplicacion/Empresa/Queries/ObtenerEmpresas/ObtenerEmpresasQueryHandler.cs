using ErrorOr;
using MediatR;
using VUCE3.Catalogos.Aplicacion.Persistencia;
using VUCE3.Catalogos.Aplicacion.Profesional.Queries.ObtenerProfesionales;

namespace VUCE3.Catalogos.Aplicacion.Empresa.Queries.ObtenerEmpresas
{
    public class ObtenerEmpresasQueryHandler : IRequestHandler<ObtenerEmpresasQuery, ErrorOr<List<Dominio.Entidades.Empresa>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerEmpresasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<List<Dominio.Entidades.Empresa>>> Handle(ObtenerEmpresasQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EmpresasRepository.ObtenerEmpresas();
        }
    }
}
