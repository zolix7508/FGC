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
        public bool lockedOnBabu;
        public short selectedBabuId;

        public int NX;
        public int NY;
        public List<mvTileGroup> groups;
        public int maxGroupId = 1, oldMaxGroupId;

        public List<mvPlayer> players { get; set; }
        public List<mvBabu> babuk { get; set; }
        public List<byte> szabadSzinek { get; set; }
        public List<mvTile> tiles { get; set; }

        public List<int> hdn = new List<int>() {4, 5, 114, 120, 123, 125};
        public List<int> nyariTabor = new List<int>() { 19, 84, 35, 112 };
        public List<int> teliTabor = new List<int>() { 56, 98, 21, 70 };
        public List<int> barlang { get; set; }

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

    private int N;
    private bool babuIsSelected { get { return selectedBabuId >= 0; } }

    private mvBabu SelectedBabu
    {
        get
        {
            if (babuk == null) return null;
            return babuk.FirstOrDefault(b => b.id == selectedBabuId);
        }
    }

    #region jatek
    internal mvResponse Action(int idx)
    {
        var resp = new mvResponse();
        ResultCode result = ResultCode.Unknown;
        if (babuIsSelected)
        {
            if (idx > -1)
            {
                result = checkIfValidMove(idx, false);
                if (result == ResultCode.Ok || result == ResultCode.LockOnBabu)
                {
                    if (result == ResultCode.LockOnBabu) lockedOnBabu = true;
                    var oldIdx = SelectedBabu.tileIdx;
                    SelectedBabu.tileIdx = idx;
                    resp.AddActionItem(ActionKind.DrawBabuk, babuk, oldIdx, players);
                    resp.AddActionItem(ActionKind.DrawBabuk, babuk, idx, players);
                    resp.AddActionItem(ActionKind.SelectBabu, this.selectedBabuId);
                    List<int> isolatedTiles;
                    if (tileLeft(oldIdx, SelectedBabu, out isolatedTiles))
                        resp.AddActionItem(ActionKind.TileRemoved, oldIdx, isolatedTiles);
                    //processIfTeliTabor(idx);
                    nextMove();
                    resp.AddActionItem(ActionKind.UpdateCurrentPlayer, players, CurrentPlayerIdx, CurrentPlayerLepes);
                }
                else
                {
                    resp.AddMsgItem("message", result, GetCurrentPlayerNick());
                }
            }
            else
                result = ResultCode.Ok; // removeBabu(SelectedBabu);
        } else if (idx > -1) {
            var tile = tiles[idx];
            if (tile.isRemovable()) {
                if (tile.isolated) {
                    RemoveIsolatedGroup(tile.group);
                    result = ResultCode.Ok;
                }
            }
        }
        if (!babuIsSelected && result == ResultCode.Unknown)
        {
            resp.AddMsgItem("doSomething", GetCurrentPlayerNick());
        }
        return resp;
    }


    void RemoveIsolatedGroup(int groupId)
    {
        var group = groups.FirstOrDefault(g => g.id == groupId);
        if (group != null)
        {
            List<int> isolatedTiles;
            group.tileIdxs.ForEach(i => RemoveTile(i, null, out isolatedTiles));
        }
        //graphics.unmarkIsolatedTiles(group.tileIdxs);
    }


    ResultCode checkIfValidMove(int idx, bool dontCheckSzomszeds)
    {
        if (!babuIsSelected) return ResultCode.BabuNotSelected;
        if (idx == -1) return ResultCode.Ok;
        var sourceTile = tiles[SelectedBabu.tileIdx];
        var targetTile = tiles[idx];
        if (sourceTile == targetTile) return ResultCode.SameTile;
        return checkIfValidTargetTile(idx, sourceTile, targetTile, false);
    }

    ResultCode checkIfValidTargetTile(int idx, mvTile sourceTile, mvTile targetTile, bool dontCheckSzomszeds)
    {
        if (!targetTile.isForBabu()) return ResultCode.InvalidTile;
        if (sourceTile.isBarlang() && targetTile.isBarlang()) return ResultCode.Ok;
        if (!dontCheckSzomszeds && (!targetTile.szomszedok.Any(sz => sz == SelectedBabu.tileIdx))) return ResultCode.NotSzomszedTile;
        if (targetTile.tileKind == TileKind.Mamut && !GetPlayer(CurrentPlayerIdx).hasFegyver) return ResultCode.NoWeapon;
        if (getBabukCountOnTile(idx, null) > 0 && !targetTile.allowsMoreBabus())
        {
            if (CurrentPlayerLepes == 0)
                return ResultCode.LockOnBabu;
            else
                return ResultCode.MoreBabusNotAllowed;
        }
        return ResultCode.Ok;
    }

    int getBabukCountOnTile(int idx, mvBabu excludeBabu)
    {
        return babuk.Count(b => b.tileIdx == idx && b != excludeBabu);
    }


    bool tileLeft(int idx, mvBabu babu, out List<int> isolatedTiles)
    {
        if ( getBabukCountOnTile(idx, babu) == 0 )
        {
            var tile = tiles[idx];
            if (tile.isRemovable()) players[babu.playerIdx].processLeszedettTile(tile);
            var tileRemoved = RemoveTile(idx, tile, out isolatedTiles);
            //if (tileRemoved) players[babu.playerIdx].processLeszedettTile(tile);
            return tileRemoved;
        }
        isolatedTiles = null;
        return false;
    }


    bool RemoveTile(int idx, mvTile tile, out List<int> isolatedTiles) {
        isolatedTiles = null;
        if (tile == null) tile = tiles[idx];
        if (!tile.isRemovable()) return false;

        foreach (int sz in tile.szomszedok) {
            for (int j=0; j<tiles[sz].szomszedok.Count; j++) 
                if (tiles[sz].szomszedok[j] == idx) {
                    tiles[sz].szomszedok.RemoveAt(j);
                    break;
                }
        }

        SetTile(tile, TileKind.Init);

        if (tile.isolated) return false;

        oldMaxGroupId = maxGroupId;
        foreach (var sz in tile.szomszedok) {
            if (tiles[sz].group <= oldMaxGroupId)
                PropagateGroup(tiles[sz], ++maxGroupId, oldMaxGroupId, sz);
        }
        
        groups = new List<mvTileGroup>();
        for (int i=0; i<tiles.Count; i++) {
            var tile2 = tiles[i];
            if (tile2.isForBabu()) {
                int n = -1;
                for (int g=0; g<groups.Count; g++) {
                    if (groups[g].id == tile.group) { n=g; break; }
                }
                if (n != -1)
                    groups[n].tileIdxs.Add(i);
                else
                    groups.Add(new mvTileGroup { id = tile.group, tileIdxs = new List<int>() {i}});
            }
        }

        foreach (var babu in babuk) {
            if (!babu.isOnMap()) continue;
            int gId = tiles[babu.tileIdx].group;
            var gr = groups.FirstOrDefault(g=>g.id == gId);
            if (gr== null) continue;
            gr.HasBabu = true;
        }

        var isolatedGroups = groups.Where(g=>!g.HasBabu);

        if (isolatedGroups.Any()) {
            var idxs = new List<int>();
            isolatedGroups.SelectMany(g=>g.tileIdxs).ToList().ForEach(i=>  {
                var tile2 = tiles[idx];
                tile2.isolated = true;
                if (tile2.isRemovable()) idxs.Add(i);
            });

            if (idxs.Any()) isolatedTiles = idxs;
        }
        return true;
    }


    void PropagateGroup(mvTile tile, int newGroupId, int oldMaxGroupId, int idx)
    {
        if (tile.group <= oldMaxGroupId && tile.group != newGroupId)
        {
            tile.group = newGroupId;
            //$('#base' + idx).text(newGroupId + ' ' + idx);
            foreach (int sz in tile.szomszedok) { PropagateGroup(tiles[sz], newGroupId, oldMaxGroupId, sz); }
            if (tile.isBarlang()) foreach (int sz in barlang) { PropagateGroup(tiles[sz], newGroupId, oldMaxGroupId, sz); }
        }
    }


    void nextMove()
    {
        CurrentPlayerLepes++;
        if (CurrentPlayerLepes == 2)
        {
            SwitchToNextPlayer(true);
            return;
        }

        var jatekosHasBabu = playerHasBabu(CurrentPlayerIdx);
        if (!jatekosHasBabu)
        {
            //var nev = currentPlayer.nev;
            SwitchToNextPlayer(false);
            //updateBoard(Localizer.playerHasNoBabu(nev));
        }
    }


        void SwitchToNextPlayer(bool dontRefreshBoard) {
            var n = CurrentPlayerIdx;
            bool playerFound = false; 
            while (!playerFound && ++n != CurrentPlayerIdx)
            {
                if (n == players.Count) n = 0;
            //newPlayer = players[n];
            playerFound = playerHasBabu(n);
        }

        if (playerFound)
            setCurrentPlayer(n);
        else
            endPhase();            

        //if (!dontRefreshBoard) updateBoard();
    }


        bool playerHasBabu(short playerIdx)
        {
            return babuk.Any(babu => babu.playerIdx == playerIdx && babu.isOnMap());
        }

        void endPhase()
        {
            //if (phase == Phase.Nyar)
            //    ittATel();
            //else
            //    endGame();
        }


    internal mvResponse babuClicked(short id)
    {
        var resp = new mvResponse();
        var babu = babuk.FirstOrDefault(b => b.id == id);
        if (babu.playerIdx != CurrentPlayerIdx)
        {
            resp.AddMsgItem("foreignBabu", GetCurrentPlayerNick(), GetPlayerNick(babu.playerIdx));
        }
        else if (lockedOnBabu && babu.id != selectedBabuId)
            resp.AddMsgItem("notTheLockedBabu", GetCurrentPlayerNick());
        else
        {
            selectedBabuId = babu.id;
            resp.AddActionItem(ActionKind.SelectBabu, selectedBabuId);
        }
        return resp;
    }
    #endregion

    public void Init(IEnumerable<IPlayer> _players)
        {
            partyPhase = PartyPhase.InitialSetup;
            this.szabadSzinek = getSzinek().ToList();
            this.players = this.getPlayers(_players).ToList();
            this.CurrentPlayerIdx = selectedBabuId = -1;
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

        void setShuffleTiles<T>(T items, IList<int> _tiles) where T : struct
        {
            var fields = typeof(T).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Type tileKindType = typeof(TileKind);
            var rdm = new Random(DateTime.Now.Millisecond);
            foreach (var field in fields)
            {
                TileKind tileKind = (TileKind)Enum.Parse(tileKindType, field.Name);
                int b = (int)field.GetValue(items);
                while (b-- > 0 && _tiles.Count != 0)
                {
                    int i = rdm.Next(_tiles.Count);
                    SetTile(_tiles[i], tileKind);
                    _tiles.RemoveAt(i);
                }
            }
            tiles.Where(t=>t.tileKind == TileKind.Mamut).ToList().ForEach(t => t.pont = 3);
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

        //mvPlayer GetCurrentPlayer()
        //{
        //    return GetPlayer(CurrentPlayerIdx);
        //}

        mvPlayer GetPlayer(short playerIdx)
        {
            if (playerIdx >= 0 && players != null)
                return players[playerIdx];

            return null;
        }

        string GetCurrentPlayerNick()
        {
            var cp = GetPlayer(CurrentPlayerIdx);
            return cp != null ? cp.Nick : String.Empty;
        }

        string GetPlayerNick(short playerIdx)
        {
            var p = GetPlayer(playerIdx);
            return p != null ? p.Nick : String.Empty;
        }
        #endregion
    }
}
