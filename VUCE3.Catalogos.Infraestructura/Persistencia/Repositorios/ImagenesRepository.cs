using Azure.Storage.Blobs;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Buffers.Text;
using VUCE3.Catalogos.Aplicacion.Persistencia.Repositorios;
using VUCE3.Catalogos.Dominio.Entidades;
using VUCE3.Catalogos.Dominio.Errores;

namespace VUCE3.Catalogos.Infraestructura.Persistencia.Repositorios
{
    public class ImagenesRepository : IImagenesRepository
    {
        private readonly CatalogosDbContext _context;
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobServiceClient;

        public ImagenesRepository(CatalogosDbContext context, IConfiguration config, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _config = config;
        }

        public async Task<ErrorOr<List<Imagenes>>> ObtenerImagenes()
        {
            return await _context.Imagenes.ToListAsync();
        }

        public async Task<ErrorOr<Imagenes>> ObtenerImagenPorId(int id)
        {
            var imagen = await _context.Imagenes.FindAsync(id);

            if (imagen is null)
            {
                return ErroresImagenes.NoEncontrada;
            }

            return imagen;
        }

        public async Task<ErrorOr<Imagenes>> CrearImagen(Imagenes imagenes)
        {
            var guid = Guid.NewGuid().ToString();

            var containerClient = _blobServiceClient.GetBlobContainerClient(_config["Blob:ContainerName"]);
            var blobClient = containerClient.GetBlobClient(guid);
            var stream = new MemoryStream(Convert.FromBase64String(imagenes.Imagen.Split(',')[1]));
            await blobClient.UploadAsync(stream, true);

            imagenes.Imagen = guid;

            var imagenCreada = await _context.Imagenes.AddAsync(imagenes);
            return imagenCreada.Entity;
        }

        public async Task<ErrorOr<Imagenes>> ActualizarImagen(Imagenes imagenes, int idImagen, IEnumerable<string> changeList)
        {
            Imagenes? config = await _context.Imagenes.FindAsync(idImagen);

            if (config is null)
            {
                return ErroresImagenes.NoEncontrada;
            }

            var listaCambios = changeList.ToList();
            if (listaCambios.Contains("Imagen"))
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_config["Blob:ContainerName"]);
                var blobClient = containerClient.GetBlobClient(config.Imagen);
                var stream = new MemoryStream(Convert.FromBase64String(imagenes.Imagen.Split(',')[1]));
                await blobClient.UploadAsync(stream, true);
                listaCambios.Remove("Imagen");
            }

            foreach (var change in listaCambios)
            {
                config.GetType().GetProperty(change)?.SetValue(config, imagenes.GetType().GetProperty(change)?.GetValue(imagenes));
            }

            return config;
        }

        public async Task<ErrorOr<Deleted>> EliminarImagen(int id)
        {
            var imagen = await _context.Imagenes.FindAsync(id);

            if (imagen is null)
            {
                return ErroresImagenes.NoEncontrada;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_config["Blob:ContainerName"]);
            var blobClient = containerClient.GetBlobClient(imagen.Imagen);
            await blobClient.DeleteAsync();

            _context.Imagenes.Remove(imagen);
            return Result.Deleted;
        }
    }
}
