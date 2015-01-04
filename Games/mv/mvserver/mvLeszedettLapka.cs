using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    public class mvLeszedettLapka
    {
        public TileKind TileKind { get; set; }
        public short Count { get; set; }

        public static void Add(List<mvLeszedettLapka> lapkak, TileKind tileKind)
        {
            if (lapkak != null)
            {
                var lapka = lapkak.FirstOrDefault(l => l.TileKind == tileKind);
                if (lapka != null) lapka.Count++;
                else lapkak.Add(new mvLeszedettLapka { TileKind = tileKind, Count = 1 });
            }
        }
    }
}
