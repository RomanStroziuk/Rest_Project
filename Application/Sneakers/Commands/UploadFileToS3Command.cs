using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Files.Commands
{
    // Команда для завантаження файлу на S3
    public record UploadFileToS3Command(
        string BucketName,
        Stream FileStream,
        string ContentType) : IRequest<bool>;

    // Обробник для команди UploadFileToS3Command
    public class UploadFileToS3CommandHandler : IRequestHandler<UploadFileToS3Command, bool>
    {
        private readonly IAmazonS3 _client;

        public UploadFileToS3CommandHandler(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<bool> Handle(UploadFileToS3Command request, CancellationToken cancellationToken)
        {
            var key = $"post_images/{Guid.NewGuid()}";

            var putRequest = new PutObjectRequest
            {
                BucketName = request.BucketName,
                Key = key,
                InputStream = request.FileStream,
                CannedACL = S3CannedACL.PublicRead,
                ContentType = request.ContentType
            };

            var response = await _client.PutObjectAsync(putRequest, cancellationToken);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}