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

        protected struct teliKovek {
            const int Bogyo = 30;
            const int Gyoker = 18;
            const int Fuszer = 5;
            const int Irha = 10;
            const int Koponya = 10;
            const int Fegyver = 3;
            const int Mamut = 12;
        }

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
        List<int> isolatedTiles;
        if (idx > -1 && tiles[idx].isolated && tiles[idx].isForBabuDI() && RemoveIsolatedGroup(idx, out isolatedTiles))
        {
            result = ResultCode.Ok;
            //resp.AddActionItem(ActionKind.RemoveIsolated, idx, isolatedTiles);
            resp.AddActionItem(ActionKind.FullStatus, this);
        }
        else if (babuIsSelected)
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
                    if (tileLeft(oldIdx, SelectedBabu, out isolatedTiles)) ;
                    processIfTeliTabor(idx, ref resp);
                    resp += nextMove();
                    resp.AddActionItem(ActionKind.FullStatus, this);
                }
                else
                {
                    resp.AddMsgItem("message", result, GetCurrentPlayerNick());
                }
            }
            else
            {
                var babu = SelectedBabu;
                isolatedTiles = RemoveBabu(SelectedBabu, ref resp);
                resp.AddActionItem(ActionKind.FullStatus, this);
                resp.AddActionItem(ActionKind.RemoveBabu, babu.id);
                result = ResultCode.Ok;
            }
        }

        if (!babuIsSelected && result == ResultCode.Unknown)
        {
            resp.AddMsgItem("doSomething", GetCurrentPlayerNick());
        }
        return resp;
    }


    bool RemoveIsolatedGroup(int idx, out List<int> isolatedTiles)
    {
        isolatedTiles = null;
        var tile = tiles[idx];
        if (tile.isRemovable())
        {
            var group = groups.FirstOrDefault(g => g.id == tile.group);
            if (group != null)
            {
                isolatedTiles = group.tileIdxs.Where(i => tiles[i].isRemovable()).ToList();
                List<int> isolatedTiles2 = null;
                group.tileIdxs.ForEach(i => RemoveTile(i, null, out isolatedTiles2, false));
                return true;
            }
            else
                return RemoveTile(idx, tile, out isolatedTiles, true);
        }
        return false;
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
        if (targetTile.tileKind == TileKind.Mamut && !GetPlayer(CurrentPlayerIdx).hasFegyver && !currentTileIsFreeWeapon() ) return ResultCode.NoWeapon;
        if (getBabukCountOnTile(idx, null) > 0 && !targetTile.allowsMoreBabus())
        {
            if (CurrentPlayerLepes == 0)
                return ResultCode.LockOnBabu;
            else
                return ResultCode.MoreBabusNotAllowed;
        }
        return ResultCode.Ok;
    }


    bool currentTileIsFreeWeapon()
    {
        return SelectedBabu.tileIdx >=0 && tiles[SelectedBabu.tileIdx].tileKind == TileKind.Fegyver && getBabukCountOnTile(SelectedBabu.tileIdx, SelectedBabu) == 0;
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
            var tileRemoved = RemoveTile(idx, tile, out isolatedTiles, false);
            //if (tileRemoved) players[babu.playerIdx].processLeszedettTile(tile);
            return tileRemoved;
        }
        isolatedTiles = null;
        return false;
    }


    List<int> RemoveBabu(mvBabu babu, ref mvResponse resp)
    {
        if (babu == null) return null;

        selectedBabuId = -1;
        var idx = babu.tileIdx;
        babu.removeFromMap();
        babuk.Remove(babu);
        ////babuk.splice(n, 1);

        var tile = tiles[idx];
        List<int> isolatedTiles;
        tileLeft(idx, babu, out isolatedTiles);

        if (tile.tileKind != TileKind.Init)
        { // tile was not removed.
            var group = groups.FirstOrDefault(g => g.id == tile.group);

            bool isolatedGroup = !babuk.Any(_babu => _babu != babu && _babu.isOnMap() && tiles[_babu.tileIdx].group == tile.group);

            if (isolatedGroup)
            {
                isolatedTiles = new List<int>();
                if (group != null)
                {
                    foreach (int gidx in group.tileIdxs)
                    {
                        var gtile = tiles[gidx];
                        gtile.isolated = true;
                        if (gtile.isRemovable()) isolatedTiles.Add(gidx);
                    }
                }
                else
                {
                    for (int gidx = 0; gidx < tiles.Count; gidx++)
                    {
                        var gtile = tiles[gidx];
                        gtile.isolated = true;
                        if (gtile.isRemovable()) isolatedTiles.Add(gidx);
                    }
                    groups = new List<mvTileGroup>() { new mvTileGroup { id = 1, tileIdxs = isolatedTiles } };
                }
            }
        }
        resp += nextMove();
        return isolatedTiles;
    }


    bool RemoveTile(int idx, mvTile tile, out List<int> isolatedTiles, bool groupAnalysisOnly) {
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

        if (!groupAnalysisOnly)
        {
            SetTile(tile, TileKind.Init);
            if (tile.isolated) return false;
        }

        oldMaxGroupId = maxGroupId;
        foreach (var sz in tile.szomszedok)
        {
            if (tiles[sz].group <= oldMaxGroupId)
                PropagateGroup(tiles[sz], ++maxGroupId, oldMaxGroupId, sz);
        }

        groups = new List<mvTileGroup>();

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile2 = tiles[i];
            if (tile2.isForBabu() || (tile2.isolated && groupAnalysisOnly && tile2.isForBabuDI()))
            {
                var gr = groups.FirstOrDefault(g => g.id == tile2.group);
                if (gr != null) gr.tileIdxs.Add(i);
                else groups.Add(new mvTileGroup { id = tile2.group, tileIdxs = new List<int>() { i } });
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
                var tile2 = tiles[i];
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


    mvResponse nextMove()
    {
        CurrentPlayerLepes++;
        if (CurrentPlayerLepes == 2)
        {
            return SwitchToNextPlayer(true);
        }

        var jatekosHasBabu = playerHasBabu(CurrentPlayerIdx);
        if (!jatekosHasBabu)
        {
            return SwitchToNextPlayer(false);
        }
        return null;
    }


    mvResponse SwitchToNextPlayer(bool dontRefreshBoard)
    {
        var n = CurrentPlayerIdx;
        bool playerFound = false;
        if (babuk.Count > 0)
        {
            while (!playerFound && ++n != CurrentPlayerIdx)
            {
                if (n == players.Count) n = 0;
                playerFound = playerHasBabu(n);
            }
        }
        if (playerFound)
            setCurrentPlayer(n);
        else
            return endPhase();

        //if (!dontRefreshBoard) updateBoard();
        return null;
    }


        bool playerHasBabu(short playerIdx)
        {
            return babuk.Any(babu => babu.playerIdx == playerIdx && babu.isOnMap());
        }


        void processIfTeliTabor(int idx, ref mvResponse resp) {
        if (phase == Phase.Nyar && teliTabor.IndexOf(idx) > -1) {
            var tile = tiles[idx];
            var currentPlayer = players[CurrentPlayerIdx];
            if (currentPlayer.ladak.IndexOf(idx) == -1)
            {
                currentPlayer.ladak.Add(idx);
                resp.AddActionItem(ActionKind.LadaDeployed, idx, currentPlayer.Szinkod);
            }
        }
    }


        mvResponse endPhase()
        {
            if (phase == Phase.Nyar)
                return ittATel();
            else
                return endGame();
        }


        mvResponse ittATel()
        {
            var resp = new mvResponse();
            babuk = new List<mvBabu>();
            phase = Phase.Tel;
            
            byte id = 0;
            var newPlayers = players.Where(p => p.ladak.Any()).ToList();
            for (short p = 0; p < players.Count(); p++)
            {
                //if (!players[p].ladak.Any()) continue;
                players[p].ladak.ForEach(l => { babuk.Add(new mvBabu { id = id++, playerIdx = p, tileIdx = l }); });
            }
            if (newPlayers.Any())
            {
                groups = new List<mvTileGroup>();
                shuffleTiles();
                CurrentPlayerLepes = 0;
                short maxMamut = 0;
                var mamutLapKak = newPlayers.SelectMany(p => p.lapkak.Where(l => l.TileKind == TileKind.Mamut));
                if (mamutLapKak.Any())
                    maxMamut = mamutLapKak.Max(m => m.Count);
                var kezdoPlayers = newPlayers.Where(p => p.lapkak.Max(l => l.Count) == maxMamut);
                CurrentPlayerIdx = (short)new Random(DateTime.Now.Millisecond).Next(kezdoPlayers.Count());
                var kezdoPlayer = newPlayers[CurrentPlayerIdx];
                for (CurrentPlayerIdx = 0; CurrentPlayerIdx < players.Count; CurrentPlayerIdx++)
                    if (players[CurrentPlayerIdx] == kezdoPlayer) break;

                players.ForEach(p => p.ladak = new List<int>());
                resp.AddActionItem(ActionKind.WinterStart, this);
            }
            else
                resp = endGame();
            return resp;
        }

        internal mvResponse endGame() { return null; }

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
            this.groups = new List<mvTileGroup>();
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
                    yield return new mvPlayer { Id = _player.Id, Nick = _player.Nick, UserId = _player.UserId, ClientStatus = ClientStatus.Connected };
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
