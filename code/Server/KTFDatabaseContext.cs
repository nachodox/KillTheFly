using Microsoft.EntityFrameworkCore;
using KillTheFly.Shared;

public class KTFDatabaseContext : DbContext
{
    public DbSet<GameEntity> Entities { get; set; }
    public DbSet<Movement> Movements { get; set; }
    public DbSet<Kill> Kills { get; set; }

    public KTFDatabaseContext(DbContextOptions<KTFDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entity mappings here if needed
        modelBuilder.Entity<GameEntity>()
            .HasKey(entity => entity.Guid);
        modelBuilder.Entity<GameEntity>()
            .Property(e => e.CreationDate)
            .HasColumnType("timestamp without time zone");
        modelBuilder.Entity<GameEntity>()
            .Property(e => e.LastAccess)
            .HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Movement>()
            .HasKey(entity => entity.Guid);
        modelBuilder.Entity<Movement>()
            .Property(e => e.MoveDate)
            .HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Kill>()
            .Ignore(kill => kill.KillerMovement)
            .HasKey(entity => entity.Guid);
        modelBuilder.Entity<Kill>()
            .Property(e => e.EventDate)
            .HasColumnType("timestamp without time zone");

    }
}
