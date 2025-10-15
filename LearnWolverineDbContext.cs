using Microsoft.EntityFrameworkCore;

namespace LearnWolverine;

public class LearnWolverineDbContext : DbContext
{
    public LearnWolverineDbContext(DbContextOptions<LearnWolverineDbContext> options)
        : base(options)
    {
    }

    public DbSet<PingEntity> Pings => Set<PingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PingEntity>(entity =>
        {
            entity.ToTable("Pings");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}

// Entity for database persistence
public class PingEntity
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
