using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class MapRepresentation
    {
        public static MapRepresentation EmptySpace(int x, int y) => 
            new()
            { Avatar = 'E', X = x, Y = y };
        
        public int X { get; set; }
        public int Y { get; set; }
        public char Avatar { get; set; }
        public string? Guid { get; set; }
        public bool IsPlayer { get; set; } = false;
    }
}
