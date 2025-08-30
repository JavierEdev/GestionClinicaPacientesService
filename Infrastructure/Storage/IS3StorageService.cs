using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace pacientes_service.Infrastructure.Storage
{
    public interface IS3StorageService
    {
        Task<(string bucket, string key, long size)> UploadAsync(
            int idPaciente, string categoria, IFormFile file, CancellationToken ct = default);

        Task<string> GetPresignedDownloadUrlAsync(string bucket, string key, TimeSpan ttl);
        Task DeleteAsync(string bucket, string key, CancellationToken ct = default);
    }
}
