using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public enum Directions
    { 
        UpLeft = 0,
        Up = 1,
        UpRight = 2,
        Left = 3,
        Center = 4,
        Right = 5,
        DownLeft = 6,
        Down = 7,
        DownRight = 8
    }
    public class Movement
    {
        public static readonly List<Directions> LEFT = new List<Directions> { Directions.UpLeft, Directions.Left, Directions.DownLeft};
        public static readonly List<Directions> UP = new List<Directions> { Directions.UpLeft, Directions.Up, Directions.UpRight };
        public static readonly List<Directions> RIGHT = new List<Directions> { Directions.UpRight, Directions.Right, Directions.DownRight };
        public static readonly List<Directions> DOWN = new List<Directions> { Directions.DownLeft, Directions.Down, Directions.DownRight};
        public Directions Direction { get; set; }
        public DateTime MoveDate { get; set; }
        public GameEntity? Entity { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
