using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Domain.Sneakers;
using Application.Common.Interfaces.Repositories;
using Application.Files;
using Microsoft.EntityFrameworkCore;
using Application.Files.Commands;
using MediatR;


namespace Api.Controllers
{
    [Route("file")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ISneakerImageRepository _sneakerImageRepository;

        public UploadController(ISender sender, ISneakerImageRepository sneakerImageRepository)
        {
            _sender = sender;
            _sneakerImageRepository = sneakerImageRepository;
            
        }
        
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid sneakerId, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadSneakerImageCommand
            {
                SneakerId = sneakerId,
                File = file
            };

            var result = await _sender.Send(command, cancellationToken);

            return result.Match<ActionResult>(
                imageUrl => Ok(new { Message = "File uploaded successfully", ImageUrl = imageUrl }),
                e => e switch
                {
                    SneakerNotFoundException => NotFound(e.Message),
                    FileUploadFailedException => StatusCode(500, e.Message),
                    _ => StatusCode(500, "An unexpected error occurred.")
                }
            );
        }
        
        [HttpDelete("{imageId:guid}")]
        public async Task<ActionResult> DeleteImage(Guid imageId, CancellationToken cancellationToken)
        {
            var command = new DeleteSneakerImageCommand { Id = imageId };
            
            var result = await _sender.Send(command, cancellationToken);

            return result.Match<ActionResult>(
                _ => NoContent(), 
                e => e switch
                {
                    SneakerImageNotFoundException => NotFound(e.Message), 
                    _ => StatusCode(500, "An unexpected error occurred.") 
                }
            );
        }
        
        [HttpPut("update")]
        public async Task<ActionResult> UpdateImage([FromForm] IFormFile file, [FromForm] Guid imageId, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var command = new UpdateSneakerImageCommand
            {
                ImageId = new SneakerImageId(imageId), 
                File = file
            };

            var result = await _sender.Send(command, cancellationToken);

            return result.Match<ActionResult>(
                updatedImage => Ok(new { Message = "Sneaker image updated successfully", ImageUrl = updatedImage.S3Path }),
                e => e switch
                {
                    SneakerImageNotFoundException => NotFound($"Image with ID {imageId} not found."),
                    FileUploadFailedException => StatusCode(500, "Failed to upload the file."),
                    _ => StatusCode(500, "An unexpected error occurred.")
                }
            );
        }
        


        
        [HttpGet("sneaker/{sneakerId:guid}")]
        public async Task<ActionResult<IEnumerable<SneakerImage>>> GetBySneakerId(Guid sneakerId, CancellationToken cancellationToken)
        {
            var images = await _sneakerImageRepository.GetBySneakerIdAsync(new SneakerId(sneakerId), cancellationToken);
            if (!images.Any())
            {
                return NotFound("No images found for this sneaker.");
            }
            return Ok(images);
        }
    }
}