using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Model;

namespace mvserver
{
    [Serializable]
    public class mvPlayer : Player
    {
        public bool szinMehet;
        public int ladak { get; set; }

        public byte fegyverek = 0;
        public List<int> mamutok;
        public List<mvLeszedettLapka> lapkak;

        public bool hasFegyver { get { return fegyverek > 0; } }

        public void processLeszedettTile(mvTile tile)
        {
            if (tile.tileKind == TileKind.Fegyver)
                fegyverek++;
            else if (tile.tileKind == TileKind.Mamut)
            {
                fegyverek--;
                mamutok.Add(((mvTilePontos)tile).Pont);
            }
            else
                mvLeszedettLapka.Add(lapkak, tile.tileKind);
        }

        public mvPlayer()
        {
            mamutok = new List<int>();
            lapkak = new List<mvLeszedettLapka>();
        }

    }
}
