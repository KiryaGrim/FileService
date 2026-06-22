using Amazon.S3;
using Amazon.S3.Model;
using FileService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Persistence.Repositories
{
    public class YandexFileRepository : IFileStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "gnosix-materials";

        public YandexFileRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task SaveAsync(string storedName, Stream fileStream, string contentType, CancellationToken ct)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = storedName,
                InputStream = fileStream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(request, ct);
        }

        public async Task DeleteAsync(string storedName, CancellationToken ct)
        {
            await _s3Client.DeleteObjectAsync(_bucketName, storedName, ct);
        }
    }
}
