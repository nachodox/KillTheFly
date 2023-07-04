using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Server.Models
{
    public class Kill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }

        [ForeignKey("Victim")]
        public Guid GameEntityId { get; set; }
        public required GameEntity Victim { get; set; }
        
        [ForeignKey("KillerMovement")]
        public Guid MovementId { get; set; }
        public virtual required Movement KillerMovement{ get; set; }

        [Column(TypeName = "timestamp without time zone")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Timestamp { get; set; }

        internal virtual Shared.Kill ToShared => new(Victim.ToShared(), KillerMovement.ToShared())
        {
            Timestamp = this.Timestamp,
            Guid = this.Guid.ToString()
        };
    }
}
