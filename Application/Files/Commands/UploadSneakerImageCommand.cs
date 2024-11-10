using Amazon.S3;
using Amazon.S3.Model;
using Domain.Sneakers;
using Microsoft.AspNetCore.Http;
using Application.Common.Interfaces.Repositories;
using MediatR;
using Application.Common;
using Microsoft.Extensions.Configuration;


namespace Application.Files.Commands;

public class UploadSneakerImageCommand : IRequest<Result<string, UploadSneakerImageException>>
{
    public required Guid SneakerId { get; init; }
    public required IFormFile File { get; init; }
}

 public class UploadSneakerImageCommandHandler : IRequestHandler<UploadSneakerImageCommand, Result<string, UploadSneakerImageException>>
    {
        private readonly IAmazonS3 _client;
        private readonly ISneakerRepository _sneakerRepository;
        private readonly ISneakerImageRepository _sneakerImageRepository;
        private readonly string _bucketName;

        public UploadSneakerImageCommandHandler(IAmazonS3 client, IConfiguration config, ISneakerRepository sneakerRepository, ISneakerImageRepository sneakerImageRepository)
        {
            _client = client;
            _bucketName = config["AWS:BucketName"];
            _sneakerRepository = sneakerRepository;
            _sneakerImageRepository = sneakerImageRepository;
        }

        public async Task<Result<string, UploadSneakerImageException>> Handle(UploadSneakerImageCommand request, CancellationToken cancellationToken)
        {
            var sneakerOption = await _sneakerRepository.GetById(new SneakerId(request.SneakerId), cancellationToken);

            var sneaker = sneakerOption.Match(
                some: s => s,
                none: () => null
            );

            if (sneaker == null)
                return new SneakerNotFoundException(request.SneakerId);

            var fileExtension = Path.GetExtension(request.File.FileName).ToLower();
            var fileKey = $"post_images/{Guid.NewGuid()}{fileExtension}";

            using var fileStream = request.File.OpenReadStream();
            var uploadResult = await UploadFileAsync(_bucketName, fileStream, request.File.ContentType, fileKey, cancellationToken);

            if (!uploadResult)
                return new FileUploadFailedException();

            var imageUrl = $"https://{_bucketName}.s3.amazonaws.com/{fileKey}";
            var sneakerImage = SneakerImage.New(new SneakerImageId(Guid.NewGuid()), new SneakerId(request.SneakerId), imageUrl);
            await _sneakerImageRepository.AddAsync(sneakerImage, cancellationToken);

            return imageUrl;
        }

        private async Task<bool> UploadFileAsync(string bucketName, Stream fileStream, string contentType, string fileKey, CancellationToken cancellationToken)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey,
                InputStream = fileStream,
                CannedACL = S3CannedACL.PublicRead,
                ContentType = contentType
            };

            try
            {
                var response = await _client.PutObjectAsync(request, cancellationToken);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

