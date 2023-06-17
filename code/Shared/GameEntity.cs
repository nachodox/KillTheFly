using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class GameEntity
    {
        public static GameEntity EmptySpace(int x, int y, char avatar) => 
            new()
            { Avatar = avatar, X = x, Y = y };
        
        public int X { get; set; }
        public int Y { get; set; }
        public char Avatar { get; set; }
        public string Guid { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? LastAccess { get; set; }
        public bool IsPlayer { get; set; } = false;
        public bool IsOnline { get; set; } = false;
    }
}
