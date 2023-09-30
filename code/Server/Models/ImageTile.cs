using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace KillTheFly.Server.Models;

public class ImageTile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; }

    [Required]
    public int LocationX { get; set; }

    [Required]
    public int LocationY { get; set; }

    [Required]
    public string ImageBase64 { get; set; } = "";
    
    [Required]
    public string Map { get; set; } = "";


    [Column(TypeName = "timestamp without time zone")]
    [DefaultValue(typeof(DateTime), "")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Timestamp { get; set; }

    internal virtual Shared.ImageTile ToShared() => new()
    {
        Guid = Guid,
        LocationX = LocationX,
        LocationY = LocationY,
        ImageBase64 = ImageBase64,
        Map = Map
    };
}
