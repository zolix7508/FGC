using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    public class mvBabu
    {
        public int tileIdx { get; set; }
        public byte id { get; set; }
        public short playerIdx { get; set; }

        internal bool isOnMap() { return tileIdx >= 0; }

        internal void removeFromMap()
        {
            //graphics.removeBabuFromMap(self);
            tileIdx = -1;
        }

        internal void putOnMap()
        {
            //graphics.putBabu(self, tileIdx);
        }

        //putOnMap();

    }
}
