using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    public class mvPlayerResult
    {
        public string Nick;
        public byte Szinkod;
        public List<mvResultItem> Points;
        public int TotalPoints
        {
            get
            {
                return Points.Where(p => p.TileKind == TileKind.Irha || p.TileKind == TileKind.Koponya || p.TileKind == TileKind.Korso || p.TileKind == TileKind.Nyaklanc).Sum(p => p.Pont) +
                    Points.Where(p => p.TileKind != TileKind.Irha && p.TileKind != TileKind.Koponya && p.TileKind != TileKind.Korso && p.TileKind != TileKind.Nyaklanc).Sum(p => p.Cnt * p.Pont);
            }
        }
    }
}
