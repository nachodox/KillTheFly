using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KillTheFly.Server.Models;

public class GameEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; }

    public string? GuidClient { get; set; } = null;

    [Required]
    [ConcurrencyCheck]
    public int X { get; set; }
    
    [Required]
    [ConcurrencyCheck]
    public int Y { get; set; }
    
    [Required]
    public int StartX { get; set; }
    
    [Required]
    public int StartY { get; set; }
    
    public char Avatar { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    [DefaultValue(typeof(DateTime), "")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreationDate { get; set; }

    [Required]
    public bool IsPlayer { get; set; } = false;

    public virtual Kill? Kill { get; set; }
    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();
    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();

    internal virtual Shared.GameEntity ToShared() => new()
    {
        Avatar = this.Avatar,
        CreationDate = this.CreationDate,
        IsPlayer = this.IsPlayer,
        Guid = this.Guid.ToString(),
        X = this.X,
        Y = this.Y
    };

    public static GameEntity EmptySpace(int x, int y, char avatar) => new()
        { Avatar = avatar, X = x, Y = y };
}
