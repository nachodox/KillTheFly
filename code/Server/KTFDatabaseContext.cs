using Microsoft.EntityFrameworkCore;
using KillTheFly.Server.Models;

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
        modelBuilder.Entity<Movement>()
            .HasOne(movement => movement.Entity)
            .WithMany(entity => entity.Movements);
        modelBuilder.Entity<Kill>()
            .HasOne(kill => kill.KillerMovement)
            .WithOne(movement => movement.Kill);
        modelBuilder.Entity<Kill>()
            .HasOne(kill => kill.Victim)
            .WithOne(victim => victim.Kill);
        modelBuilder.Entity<Access>()
            .HasOne(access => access.Entity)
            .WithMany(entity => entity.Accesses);
        //modelBuilder.Entity<GameEntity>()
        //    .Property(e => e.CreationDate)
        //    .HasColumnType("timestamp without time zone");
        //modelBuilder.Entity<Access>()
        //    .Property(e => e.Timestamp)
        //    .HasColumnType("timestamp without time zone");
        //modelBuilder.Entity<Movement>()
        //    .Property(e => e.Timestamp)
        //    .HasColumnType("timestamp without time zone");
        //modelBuilder.Entity<Kill>()
        //    .Property(e => e.Timestamp)
        //    .HasColumnType("timestamp without time zone");

    }
}
