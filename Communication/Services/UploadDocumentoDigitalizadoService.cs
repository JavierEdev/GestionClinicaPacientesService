using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;
using pacientes_service.Infrastructure.MySql;
using pacientes_service.Infrastructure.Storage;

namespace pacientes_service.Communication.Services
{
    public class UploadDocumentoDigitalizadoService
    {
        private readonly AppDbContext _db;
        private readonly IS3StorageService _s3;

        public UploadDocumentoDigitalizadoService(AppDbContext db, IS3StorageService s3)
        {
            _db = db; _s3 = s3;
        }

        public async Task<int> HandleAsync(
            int idPaciente,
            string categoria,
            IFormFile file,
            string? notas,
            CancellationToken ct = default)
        {
            var ok = await _db.Pacientes.AsNoTracking().AnyAsync(p => p.IdPaciente == idPaciente, ct);
            if (!ok) throw new ArgumentException("Paciente no existe.");

            categoria = (categoria ?? "").ToLowerInvariant();
            if (categoria is not ("dpi" or "resultado" or "seguro"))
                throw new ArgumentException("Categoría inválida. Use 'dpi', 'resultado' o 'seguro'.");

            var ctType = (file.ContentType ?? "").ToLowerInvariant();
            var isPdf = ctType == "application/pdf";
            var isImg = ctType.StartsWith("image/");
            if (!isPdf && !isImg)
                throw new ArgumentException("Solo se permiten PDF o imágenes (PNG/JPG).");

            var (bucket, key, size) = await _s3.UploadAsync(idPaciente, categoria, file, ct);

            var ahora = DateTime.UtcNow;

            var registro = new Imagenologia
            {
                IdPaciente = idPaciente,
                TipoImagen = "documento",
                Categoria = categoria,
                ImagenUrl = $"s3://{bucket}/{key}",
                S3Bucket = bucket,
                S3Key = key,
                ContentType = file.ContentType ?? "application/octet-stream",
                TamanoBytes = size,
                Notas = notas,
                FechaEstudio = ahora.Date,
                FechaDocumento = ahora,
                MedicoSolicitante = null,
                NombreArchivoOriginal = file.FileName,
            };

            _db.Imagenologia.Add(registro);
            await _db.SaveChangesAsync(ct);
            return registro.IdImagen;
        }
    }
}
