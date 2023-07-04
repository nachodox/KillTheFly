using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KillTheFly.Server.Models;

public class Movement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; }

    [Required]
    public Shared.Directions Direction { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Timestamp { get; set; }

    //[ForeignKey(nameof(GameEntity))]
    public required GameEntity Entity { get; set; }
        
    [Required]
    public int StartX { get; set; }
        
    [Required]
    public int StartY { get; set; }
        
    [Required]
    public int EndX { get; set; }
        
    [Required]
    public int EndY { get; set; }

    public virtual Kill? Kill { get; set; }
    internal virtual Shared.Movement ToShared() => new()
    {
        Direction = this.Direction,
        EndX = this.EndX,
        EndY = this.EndY,
        StartX = this.StartX,
        StartY = this.StartY,
        MoveDate = this.Timestamp,
        Guid = this.Guid.ToString(),
        Entity = this.Entity?.ToShared()
    };
}

