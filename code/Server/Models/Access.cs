using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Server.Models
{
    public class Access
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        [DefaultValue(typeof(DateTime), "")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Timestamp { get; set; }

        //[ForeignKey(nameof(GameEntity))]
        public required GameEntity Entity { get; set;}
    }
}
