using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class Kill
    {
        public string Guid { get; set; } = string.Empty;
        public GameEntity Victim { get; set; }
        public DateTime Timestamp { get; set; }
        public Movement KillerMovement{ get; set; }
        public Kill(GameEntity victim, Movement killerMovement)
        {
            Victim = victim;
            Timestamp = DateTime.Now;
            KillerMovement = killerMovement;
        }
    }
}
