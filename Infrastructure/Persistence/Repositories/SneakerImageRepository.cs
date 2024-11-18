using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Sneakers;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class SneakerImageRepository : ISneakerImageRepository
{
    private readonly ApplicationDbContext _context;

    public SneakerImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SneakerImage> AddAsync(SneakerImage sneakerImage, CancellationToken cancellationToken)
    {
        await _context.SneakerImages.AddAsync(sneakerImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sneakerImage;
    }

    public async Task<SneakerImage> GetByIdAsync(SneakerImageId id, CancellationToken cancellationToken)
    {
        return await _context.SneakerImages
            .FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SneakerImage>> GetBySneakerIdAsync(SneakerId sneakerId, CancellationToken cancellationToken)
    {
        return await _context.SneakerImages
            .Where(si => si.SneakerId == sneakerId)
            .ToListAsync(cancellationToken);
    }
        
    public async Task<bool> DeleteAsync(SneakerImageId id, CancellationToken cancellationToken)
    {
        var sneakerImage = await _context.SneakerImages.FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
        if (sneakerImage == null)
        {
            return false;
        }

        _context.SneakerImages.Remove(sneakerImage);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<SneakerImage> UpdateAsync(SneakerImage sneakerImage, CancellationToken cancellationToken)
    {
        _context.SneakerImages.Update(sneakerImage);
        await _context.SaveChangesAsync(cancellationToken);
        return sneakerImage;
    }
    
}
