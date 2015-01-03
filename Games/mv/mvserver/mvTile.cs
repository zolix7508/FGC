using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    public class mvTile
    {
        public TileKind tileKind = TileKind.Init;
        public int group = 1;
        public bool isolated;
        public List<int> szomszedok;

        public bool isRandom()
        {
            return tileKind != TileKind.Hidden && tileKind != TileKind.NyariTabor && tileKind != TileKind.TeliTabor;
        }

        public bool isBarlang() { return tileKind == TileKind.Barlang; }

        public bool isRemovable()
        {
            return isRandom() && tileKind != TileKind.Barlang && tileKind != TileKind.Ut && tileKind != TileKind.Init;
        }

        public bool isForBabu() { 
            return tileKind != TileKind.Init && tileKind != TileKind.Hidden && !isolated; 
        }

        public bool allowsMoreBabus()
        {
            return tileKind == TileKind.Barlang || tileKind == TileKind.NyariTabor || tileKind == TileKind.TeliTabor;
        }

        public void Init()
        {
            tileKind = TileKind.Init;
            //szomszedok = new List<int>();
        }
    }
}
