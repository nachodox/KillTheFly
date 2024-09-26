using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillTheFly.Shared
{
    public class CreateTileDto
    {
        public string ImageBase64 { get; set; } = "";
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string Map { get; set; } = "";
    }
}
