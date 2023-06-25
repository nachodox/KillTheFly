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
        public static readonly Dictionary<Directions, string> MusicalNotes = new(){
            {Directions.UpLeft, "C4"},
            {Directions.Up, "D4"},
            {Directions.UpRight, "E4"},
            {Directions.Left, "F4"},
            {Directions.Center, "Plop"},
            {Directions.Right, "G4"},
            {Directions.DownLeft, "A4"},
            {Directions.Down, "B4"},
            {Directions.DownRight, "C5"}
        };
        public static readonly List<Directions> LEFT = new() { 
            Directions.UpLeft, Directions.Left, Directions.DownLeft
        };
        public static readonly List<Directions> UP = new() { 
            Directions.UpLeft, Directions.Up, Directions.UpRight 
        };
        public static readonly List<Directions> RIGHT = new() { 
            Directions.UpRight, Directions.Right, Directions.DownRight };
        public static readonly List<Directions> DOWN = new() { 
            Directions.DownLeft, Directions.Down, Directions.DownRight};
        public Directions Direction { get; set; }
        public DateTime MoveDate { get; set; }
        public GameEntity? Entity { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
