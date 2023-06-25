using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class GameSnapshot
    {
        public ScoreBoard? Board { get; set; }
        public IEnumerable<GameEntity> Map { get; set; } = new List<GameEntity>();
    }
}
