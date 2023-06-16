using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class MapSnapshot
    {
        public int Foes { get; set; }
        public int GameTime { get; set; }
        public int MilisecondsPast { get; set; }
        public IEnumerable<MapRepresentation> Map { get; set; } = new List<MapRepresentation>();
    }
}
