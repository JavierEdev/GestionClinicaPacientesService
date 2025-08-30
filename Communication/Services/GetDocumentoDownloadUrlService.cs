using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pacientes_service.Infrastructure.MySql;
using pacientes_service.Infrastructure.Storage;

namespace pacientes_service.Communication.Services
{
    public class GetDocumentoDownloadUrlService
    {
        private readonly AppDbContext _db;
        private readonly IS3StorageService _s3;

        public GetDocumentoDownloadUrlService(AppDbContext db, IS3StorageService s3)
        { _db = db; _s3 = s3; }

        public async Task<string> HandleAsync(int idPaciente, int idImagen, TimeSpan ttl, CancellationToken ct = default)
        {
            var doc = await _db.Imagenologia.AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdImagen == idImagen && d.IdPaciente == idPaciente, ct);

            if (doc is null)
                throw new ArgumentException("Documento no encontrado.");

            if (string.IsNullOrWhiteSpace(doc.S3Bucket) || string.IsNullOrWhiteSpace(doc.S3Key))
                throw new InvalidOperationException("El documento no tiene ubicación S3 registrada.");

            return await _s3.GetPresignedDownloadUrlAsync(doc.S3Bucket!, doc.S3Key!, ttl);
        }
    }
}
