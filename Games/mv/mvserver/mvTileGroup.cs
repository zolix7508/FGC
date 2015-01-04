using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    public class mvTileGroup
    {
        public int id { get; set; }
        public List<int> tileIdxs { get; internal set; }
        public bool HasBabu { get; set; }

        public mvTileGroup()
        {
            tileIdxs = new List<int>();
        }
    }
}
