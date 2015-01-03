using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;

namespace mvserver
{
    public class mvParty 
    {
        public PartyPhase partyPhase { get; set; }
        public Phase phase { get; set; }
        public short CurrentPlayerIdx { get; set; }
        public short CurrentPlayerLepes { get; set; }
        protected bool lockedOnBabu;
        protected int selectedBabuId;

        public int NX;
        public int NY;
        protected List<mvTileGroup> groups;
        protected int maxGroupId = 1, oldMaxGroupId;

        public List<mvPlayer> players { get; set; }
        public List<mvBabu> babuk { get; set; }
        public List<byte> szabadSzinek { get; set; }
        public List<mvTile> tiles { get; set; }

        public List<int> hdn = new List<int>() {4, 5, 114, 120, 123, 125};
        public List<int> nyariTabor = new List<int>() { 19, 84, 35, 112 };
        public List<int> teliTabor = new List<int>() { 56, 98, 21, 70 };

        protected struct nyariKovek
        {
            const int Bogyo = 30;
            const int Ut = 19;
            const int Gyoker = 18;
            const int Fuszer = 5;
            const int Korso = 10;
            const int Nyaklanc = 10;
            const int Fegyver = 12;
            const int Mamut = 3;
            const int Barlang = 5;
        }
    //var teliKovek = { Bogyo: 30, Gyoker: 18, Fuszer: 5, Irha: 10, Koponya: 10, Fegyver: 3, Mamut: 12 };
        protected struct teliKovek { }
    protected List<int> barlang {get; set; }

    private int N;

        public void Init(IEnumerable<IPlayer> _players)
        {
            partyPhase = PartyPhase.InitialSetup;
            this.szabadSzinek = getSzinek().ToList();
            this.players = this.getPlayers(_players).ToList();
        }

        public void StartPlaying()
        {
            AdjustMapLayout();
            this.phase = Phase.Nyar;
            
            byte id=0;
            for (short p = 0; p < players.Count; p++)
            {
                nyariTabor.ForEach(x =>
                {
                    babuk.Add(
                        new mvBabu { tileIdx = x, playerIdx = p, id = id++ });
                });
            }
            shuffleTiles();
            this.barlang = new List<int>();
            for (int i=0; i<tiles.Count; i++) if (tiles[i].isBarlang()) barlang.Add(i);
            setCurrentPlayer((short)new Random(DateTime.Now.Millisecond).Next(players.Count));
        }


        public IEnumerable<byte> getSzinek()
        {
            var enumType = typeof(Szin);
            foreach (string kodStr in Enum.GetNames(enumType))
            {
                var kod = Enum.Parse(enumType, kodStr);
                yield return (byte)kod;
            }
        }

        #region Helper Methods

        void shuffleTiles()
        {
            var arr = new List<int>();

            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].isolated = false;
                if (tiles[i].isRandom()) arr.Add(i);
            }

            nyariKovek _nyariKovek;
            teliKovek _teliKovek;

            if (phase == Phase.Nyar) setShuffleTiles<nyariKovek>(_nyariKovek, arr);
            else if (phase == Phase.Tel) setShuffleTiles<teliKovek>(_teliKovek, arr);
        }

        void setShuffleTiles<T>(T items, IList<int> _tiles) where T:struct {
            var fields = typeof(T).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Type tileKindType = typeof(TileKind);
            var rdm = new Random(DateTime.Now.Millisecond);
            foreach (var field in fields)
            {
                TileKind tileKind = (TileKind)Enum.Parse(tileKindType, field.Name);
                int b = (int)field.GetValue(items);
                while (b-- > 0 && _tiles.Count != 0 ) {
                    int i = rdm.Next(_tiles.Count);
                    SetTile(_tiles[i], tileKind);
                    _tiles.RemoveAt(i);
                }
            }
        }

        void AdjustMapLayout()
        {
            this.partyPhase = PartyPhase.Playing;
            N = NX * NY;
            tiles = new List<mvTile>(N);
            for (int i = 0; i < N; i++) tiles.Add(new mvTile());
            babuk = new List<mvBabu>(players.Count * mvCommon.maxBabu);

            hdn.ForEach(t => SetTile(t, TileKind.Hidden));
            nyariTabor.ForEach(t => SetTile(t, TileKind.NyariTabor));
            teliTabor.ForEach(t => SetTile(t, TileKind.TeliTabor));
            setSzomszedok();
        }

        void setSzomszedok()
        {
            for (int tileIdx = 0; tileIdx < tiles.Count; tileIdx++)
            {
                var tile = tiles[tileIdx];
                if (tile.tileKind != TileKind.Hidden)
                {
                    int y = (int)Math.Floor((double)tileIdx / NX);
                    int x = tileIdx % NX;
                    int parosy = (y % 2) == 0 ? 1 : 0;
                    tile.szomszedok = new List<int>();
                    if (y > 1) tile.szomszedok.Add((y - 2) * NX + x);
                    if (y > 0 && x + parosy < NX) tile.szomszedok.Add((y - 1) * NX + x + parosy);
                    if (y < NY - 1 && x + parosy < NX) tile.szomszedok.Add((y + 1) * NX + x + parosy);
                    if (y < NY - 2) tile.szomszedok.Add((y + 2) * NX + x);
                    int newX = x - 1 + parosy;
                    if (newX >= 0 && newX < NX)
                    {
                        if (y < NY - 1) tile.szomszedok.Add((y + 1) * NX + newX);
                        if (y > 0) tile.szomszedok.Add((y - 1) * NX + newX);
                    }
                    tile.szomszedok = tile.szomszedok.Where(t2 => tiles[t2].tileKind != TileKind.Hidden).ToList();
                }
            }
        }

        void setCurrentPlayer(short playerIdx)
        {
            CurrentPlayerIdx = playerIdx;
            CurrentPlayerLepes = 0;
            lockedOnBabu = false;
            //deselectCurrentBabu();
            selectedBabuId = -1;
            //updateBoard();
        }

        void SetTile(int idx, TileKind tileKind) { tiles[idx].tileKind = tileKind; }
        void SetTile(mvTile tile, TileKind tileKind) { tile.tileKind = tileKind; }

        IEnumerable<mvPlayer> getPlayers(IEnumerable<IPlayer> players)
        {
            if (players != null)
                foreach (var _player in players)
                    yield return new mvPlayer { Id = _player.Id, Nick = _player.Nick, UserId = _player.UserId, ladak = mvCommon.maxLada, ClientStatus = ClientStatus.Connected };
        }

        #endregion
    }
}
