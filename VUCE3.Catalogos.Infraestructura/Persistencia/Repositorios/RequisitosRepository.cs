using Azure.Storage.Blobs;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VUCE3.Catalogos.Aplicacion;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class RequisitosRepository: IRequisitosRepository
    {
        private readonly CatalogosDbContext _context;
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobStorageService;
        private const string ContainerName = "Blob:ContainerName";

        public RequisitosRepository(CatalogosDbContext context, IConfiguration config, BlobServiceClient blobStorageService)
        {
            _context = context;
            _config = config;
            _blobStorageService = blobStorageService;
        }

        public async Task<ErrorOr<List<Requisito>>> ObtenerRequisitos()
        {
            return await _context.Requisitos.ToListAsync();
        }

        public async Task<ErrorOr<Requisito>> ObtenerRequisitoPorId(int Id)
        {
            var requisito = await _context.Requisitos.FindAsync(Id);

            if (requisito is null)
            {
                return ErroresRequisito.RequisitoNoEncontrado;
            }

            return requisito;
        }

        public async Task<ErrorOr<Requisito>> CrearRequisito(Requisito requisito)
        {
            bool isGuid = Guid.TryParse(requisito.ImagenRequisito, out Guid _);

            if (!string.IsNullOrWhiteSpace(requisito.ImagenRequisito) && !isGuid)
            {
                var containerClient = _blobStorageService.GetBlobContainerClient(_config[ContainerName]);

                var guidIdentification = Guid.NewGuid().ToString();
                var blobClient = containerClient.GetBlobClient(guidIdentification);
                var stream = new MemoryStream(Convert.FromBase64String(requisito.ImagenRequisito.Split(',')[1]));
                await blobClient.UploadAsync(stream, true);
                requisito.ImagenRequisito = guidIdentification;
            }
            
            var requisitoCreado = await _context.Requisitos.AddAsync(requisito);
            return requisitoCreado.Entity;
        }

        public async Task<ErrorOr<Requisito>> ActualizarRequisito(Requisito requisito, int idRequisito, IEnumerable<string> changeList)
        {
            Requisito? config = await _context.Requisitos.FindAsync(idRequisito);

            if (config is null)
            {
                return ErroresRequisito.RequisitoNoEncontrado;
            }

            bool isGuid = Guid.TryParse(requisito.ImagenRequisito, out Guid _);

            if (!string.IsNullOrWhiteSpace(requisito.ImagenRequisito) && !isGuid)
            {
                var containerClient = _blobStorageService.GetBlobContainerClient(_config[ContainerName]);
                var blobClient = containerClient.GetBlobClient(config.ImagenRequisito);
                var stream = new MemoryStream(Convert.FromBase64String(requisito.ImagenRequisito.Split(',')[1]));
                await blobClient.UploadAsync(stream, true);
                requisito.ImagenRequisito = config.ImagenRequisito;
            }

            foreach (var change in changeList)
            {
                config.GetType().GetProperty(change)?.SetValue(config, requisito.GetType().GetProperty(change)?.GetValue(requisito));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarRequisito(int id)
        {
            var requisito = await _context.Requisitos.FindAsync(id);

            if (requisito is null)
            {
                return ErroresRequisito.RequisitoNoEncontrado;
            }

            if (requisito.ImagenRequisito != null)
            {
                var containerClient = _blobStorageService.GetBlobContainerClient(_config[ContainerName]);
                var blobClient = containerClient.GetBlobClient(requisito.ImagenRequisito);
                await blobClient.DeleteIfExistsAsync();
            }

            _context.Requisitos.Remove(requisito);
            return Result.Deleted;
        }

        public async Task<ErrorOr<bool>> ValidarRequisito(int idRequisito, string codigo, string version, int idPais, int idInstitucion )
        {
            var requisitos = await _context.Requisitos
                .Where(r => r.Id != idRequisito
                && r.IdPais == idPais
                && r.IdInstitucion == idInstitucion)
                .ToListAsync();

            var existe = requisitos.Exists(c => Validadores.NormalizarString(c.Codigo) == Validadores.NormalizarString(codigo) && Validadores.NormalizarString(c.Version) == Validadores.NormalizarString(version));
            return existe;
        }
    }
}
