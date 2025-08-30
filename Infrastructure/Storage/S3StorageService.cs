using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace pacientes_service.Infrastructure.Storage
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly S3Options _opt;

        public S3StorageService(IAmazonS3 s3, IOptions<S3Options> opt)
        {
            _s3 = s3;
            _opt = opt.Value;
        }

        public async Task<(string bucket, string key, long size)> UploadAsync(
            int idPaciente, string categoria, IFormFile file, CancellationToken ct = default)
        {
            var fileName = Path.GetFileName(file.FileName);
            var ext = Path.GetExtension(fileName);
            var key = $"{_opt.BaseFolder}/paciente-{idPaciente}/{categoria}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{ext}";

            using var stream = file.OpenReadStream();

            var req = new PutObjectRequest
            {
                BucketName = _opt.Bucket,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            };

            var resp = await _s3.PutObjectAsync(req, ct);
            if ((int)resp.HttpStatusCode >= 300)
                throw new InvalidOperationException("Error subiendo archivo a S3.");

            return (_opt.Bucket, key, file.Length);
        }

        public Task<string> GetPresignedDownloadUrlAsync(string bucket, string key, TimeSpan ttl)
        {
            var req = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.UtcNow.Add(ttl),
                Verb = HttpVerb.GET
            };

            var url = _s3.GetPreSignedURL(req);
            return Task.FromResult(url);
        }

        public async Task DeleteAsync(string bucket, string key, CancellationToken ct = default)
        {
            await _s3.DeleteObjectAsync(bucket, key, ct);
        }
    }
}
