using Microsoft.EntityFrameworkCore;

namespace LearnWolverine;

public class PingRepository
{
    private readonly LearnWolverineDbContext _context;

    public PingRepository(LearnWolverineDbContext context)
    {
        _context = context;
    }

    public void Store(Ping ping)
    {
        var entity = new PingEntity
        {
            Id = ping.Id,
            Message = ping.Message,
            CreatedAt = ping.CreatedAt
        };
        
        _context.Pings.Add(entity);
        // Wolverine will automatically call SaveChangesAsync after the handler completes
    }

    public async Task<IEnumerable<Ping>> GetPingsAsync()
    {
        var entities = await _context.Pings
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        
        return entities.Select(e => new Ping(e.Id, e.Message, e.CreatedAt));
    }

    public async Task<Ping?> GetPingAsync(Guid id)
    {
        var entity = await _context.Pings.FindAsync(id);
        
        return entity != null 
            ? new Ping(entity.Id, entity.Message, entity.CreatedAt) 
            : null;
    }
}

public record Ping(Guid Id, string Message, DateTime CreatedAt = default)
{
    public Ping(Guid id, string message)
        : this(id, message, DateTime.UtcNow) { }
};

