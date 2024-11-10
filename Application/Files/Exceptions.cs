using System;
using Domain.Sneakers;


namespace Application.Files
{
    public abstract class UploadSneakerImageException : Exception
    {
        protected UploadSneakerImageException(string message) : base(message) { }
    }
    
    public class SneakerImageNotFoundException : UploadSneakerImageException
    {
        public SneakerImageNotFoundException(SneakerImageId imageId) 
            : base($"Sneaker image with ID {imageId} not found.") { }
    }

    public class SneakerNotFoundException : UploadSneakerImageException
    {
        public SneakerNotFoundException(Guid sneakerId) 
            : base($"Sneaker with ID {sneakerId} not found.") { }
    }

    public class FileUploadFailedException : UploadSneakerImageException
    {
        public FileUploadFailedException() 
            : base("Failed to upload the file to S3.") { }
    }
}