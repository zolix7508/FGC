using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    public class mvResultItem
    {
        public TileKind TileKind;
        public int Pont;
        public int Cnt;
    }
}
