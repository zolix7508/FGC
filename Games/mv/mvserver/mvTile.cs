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
        //public Dictionary<string, object> attrs;

        internal bool isRandom()
        {
            return tileKind != TileKind.Hidden && tileKind != TileKind.NyariTabor && tileKind != TileKind.TeliTabor;
        }

        internal bool isBarlang() { return tileKind == TileKind.Barlang; }

        internal bool isRemovable()
        {
            return isRandom() && tileKind != TileKind.Barlang && tileKind != TileKind.Ut && tileKind != TileKind.Init;
        }

        // DI stands for Disregard Isolation
        internal bool isForBabuDI()
        { 
            return tileKind != TileKind.Init && tileKind != TileKind.Hidden; 
        }

        internal bool isForBabu()
        { 
            return tileKind != TileKind.Init && tileKind != TileKind.Hidden && !isolated; 
        }

        internal bool allowsMoreBabus()
        {
            return tileKind == TileKind.Barlang || tileKind == TileKind.NyariTabor || tileKind == TileKind.TeliTabor;
        }

        internal void Init()
        {
            tileKind = TileKind.Init;
            //szomszedok = new List<int>();
        }

        public short pont { get; set; }

        //internal void AddAttr(string key, object value)
        //{
        //    if (attrs == null) attrs = new Dictionary<string, object>();
        //    attrs.Add(key, value);
        //}
    }
}
