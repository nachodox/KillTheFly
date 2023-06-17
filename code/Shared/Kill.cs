using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class Kill
    {
        public GameEntity Victim { get; set; }
        public DateTime EventDate { get; set; }
        public Movement KillerMovement{ get; set; }
        public Kill(GameEntity victim, Movement killerMovement)
        {
            Victim = victim;
            EventDate = DateTime.Now;
            KillerMovement = killerMovement;
        }
    }
}
