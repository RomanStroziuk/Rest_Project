using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Sneakers;
using MediatR;
using Optional;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Files.Commands
{
    public class UpdateSneakerImageCommand : IRequest<Result<SneakerImage, Exception>>
    {
        public SneakerImageId ImageId { get; init; }  // Ідентифікатор картинки
        public IFormFile File { get; init; }  // File to be uploaded
    }

   public class UpdateSneakerImageCommandHandler : IRequestHandler<UpdateSneakerImageCommand, Result<SneakerImage, Exception>>
{
    private readonly ISneakerImageRepository _sneakerImageRepository;
    private readonly IAmazonS3 _s3Client;  // Injected client
    private readonly string _bucketName;

    public UpdateSneakerImageCommandHandler(ISneakerImageRepository sneakerImageRepository, IConfiguration config, IAmazonS3 s3Client)
    {
        _sneakerImageRepository = sneakerImageRepository;
        _s3Client = s3Client;
        _bucketName = config["AWS:BucketName"];
    }

    public async Task<Result<SneakerImage, Exception>> Handle(UpdateSneakerImageCommand request, CancellationToken cancellationToken)
    {
        var existingImage = await _sneakerImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
        if (existingImage == null)
        {
            return new SneakerImageNotFoundException(request.ImageId);
        }

        var newImageUrl = await UploadFileToS3(request.File);

        existingImage.UpdateImageUrl(newImageUrl);
        var updatedImage = await _sneakerImageRepository.UpdateAsync(existingImage, cancellationToken);

        return updatedImage;
    }

    private async Task<string> UploadFileToS3(IFormFile file)
    {
        var key = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        using (var stream = file.OpenReadStream())
        {
            var uploadRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead,
            };

            var response = await _s3Client.PutObjectAsync(uploadRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return $"https://{_bucketName}.s3.amazonaws.com/{key}";
            }
            else
            {
                throw new Exception("Failed to upload file to S3.");
            }
        }
    }
}

}
