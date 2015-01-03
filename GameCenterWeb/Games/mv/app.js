
var ctx, mv;

var TileKind = {};
TileKind.Init = 0;
TileKind.Hidden = 1;
TileKind.Ut = 2;
TileKind.Barlang = 3;
TileKind.NyariTabor = 4;
TileKind.TeliTabor = 5;
//TileKind.Empty = 6;
TileKind.Bogyo = 7;
TileKind.Fuszer = 8;
TileKind.Gyoker = 9;
TileKind.Korso = 10;
TileKind.Nyaklanc = 11;
TileKind.Irha = 12;
TileKind.Koponya = 13;
TileKind.Fegyver = 14;
TileKind.Mamut = 15;

var a = [];
for (var kind in TileKind) a.push(kind);
TileKind.tileKindNames = a.slice();

SzinKodok = ['1', '2', '3', '4'];

function Tile() {
    var self = this;
    self.tileKind = TileKind.Init;
    self.group = 1;
    self.isolated;

    self.isRandom = function () { return [TileKind.Hidden, TileKind.NyariTabor, TileKind.TeliTabor].indexOf(self.tileKind) < 0 };
    self.isBarlang = function () { return self.tileKind == TileKind.Barlang };
    self.isRemovable = function () { return self.isRandom() & [TileKind.Barlang, TileKind.Ut, TileKind.Init].indexOf(self.tileKind) < 0 };
    self.isForBabu = function () { return self.tileKind != TileKind.Init && self.tileKind != TileKind.Hidden && !self.isolated };
    self.allowsMoreBabus = function() { return [TileKind.Barlang, TileKind.NyariTabor, TileKind.TeliTabor].indexOf() > -1 };
}

function Babu(tileIdx, jatekos, id) {
    var self = this;
    self.tileIdx = tileIdx;
    self.jatekos = jatekos;
    self.id = id;

    self.isOnMap = function () { return self.tileIdx >= 0 };

    self.removeFromMap = function () {
        graphics.removeBabuFromMap(self);
        self.tileIdx = -1;
    }

    var putOnMap = function () {
        graphics.putBabu(self, tileIdx);
    };

    putOnMap();
}

function Jatekos(szin, nev) {
    var self = this;
    self.szin = szin;
    self.nev = nev;
    self.fegyverek = 0, self.mamutok = [];
    self.lapkak = [];

    self.processLeszedettTile = function(tile) {
        if (tile.tileKind == TileKind.Fegyver)
            self.fegyverek++;
        else if (tile.tileKind == TileKind.Mamut) {
            self.fegyverek--;
            self.mamutok.push(tile);
        } else
            self.lapkak.push(tile);
    }
}


function App() {
    var self = this;
    self.partyId;
    self.mvParty;
    //self.jatekos = {};


    self.R = 50;
    self.NX = 6;
    self.NY = 21;
    self.maxBabuk = 4;

    var groups = [];
    var maxGroupId = 1, oldMaxGroupId;
    var N = self.NX * self.NY;
    var PII = 2 * Math.PI - 0.001;
    var u = Math.PI / 3;
    var ddx = self.R * Math.cos(u);
    var ddy = self.R * Math.sin(u);
    self.ddy = ddy;
    var tiles = new Array(N);
    self.tiles = tiles;
    var nPlayers;
    var players;
    var nyar = true;
    var babuk = [];

    var hdn = [4, 5, 114, 120, 123, 125];
    var nyariTabor = [19, 84, 35, 112];
    var teliTabor = [56, 98, 21, 70];

    var nyariKovek = { Bogyo: 30, Ut: 19, Gyoker: 18, Fuszer: 5, Korso: 10, Nyaklanc: 10, Fegyver: 12, Mamut: 3, Barlang: 5 };
    var teliKovek = { Bogyo: 30, Gyoker: 18, Fuszer: 5, Irha: 10, Koponya: 10, Fegyver: 3, Mamut: 12 };
    var barlang = [];

    var selectedBabu, currentPlayer, currentPhase, currentPlayerLepes;

    var boardElem, message, processing;

    self.getData = function (info) {
        return info ? {
            partyPhase: info.partyPhase,
            szabadSzinek: info.szabadSzinek,
            currentPlayer: info.currentPlayer,
            currentPlayerLepes: info.currentPlayerLepes,
            players: info.players,
            message: info.message,
        } : null;
    };

    function updateBoard(info) {
        var sc = angular.element(boardElem).scope();
        if (sc) {
            //if (isString(info)) message = msg;

            sc.$apply(function () {
                //sc.exData( self.getData() );
                sc.exData(info);
            });
        }
    }
    self.updateBoard = updateBoard;

    self.setPlayerStatus = function (nick, playerId, online) {
        if (!self.mvParty) return;
        var player;
        $.each(self.mvParty.players, function (i, x) {
            if (x.Id == playerId) { player = x; return false;}
        });
        if (player == null) return;
        player.Nick = nick;
        player.ClientStatus = online ? 1 : 2;
    };

    self.szinSelected = function (szin) {
        self.mvParty.jatekos.Szinkod = szin;
        mv.server.szinSelected(szin);
    };

    self.szinMehet = function() {
        mv.server.szinMehet();
    }

    self.shuffleTiles = function () {
        var arr = [];
        tiles.forEach(function (tile, i) {
            tile.isolated = false;
            if (tile.isRandom()) arr.push(i);
        });

        var kovek = currentPhase == Phase.Nyar ? nyariKovek : teliKovek;
        for (var kind in kovek) {
            var tileKindId = eval('TileKind.' + kind);
            var b = kovek[kind];
            while (b-- && arr.length) {
                var i = Math.floor(arr.length * Math.random());
                self.setTile(arr[i], tileKindId, kind);
                arr.splice(i, 1);
            }
        }
    };

    function getCoords(x0, y0, r) {
        var coords = '';
        for (var i = 0; i < PII; i += u) {
            var x = x0 + r * Math.cos(i);
            var y = y0 + r * Math.sin(i);
            coords += Math.round(x) + ',' + Math.round(y) + ',';
        }
        return coords;
    }

    function getH6area(x0, y0, i) {
        return '<area shape="poly" coords="' + getCoords(x0, y0, self.R) + '" href="javascript:void(0)" data-tile-id="' + i + '" >';
    }

    function h6(x0, y0, deg) {
        ctx.beginPath();
        ctx.moveTo(x0 + self.R, y0);
        for (var i = deg; i < 2 * Math.PI; i += deg) {
            ctx.lineTo(x0 + self.R * Math.cos(i), y0 + self.R * Math.sin(i));
        }
        ctx.stroke();
    }

    getTileCoords = function (tileIdx) {
        var y = Math.floor(tileIdx / self.NX), x = tileIdx % self.NX;
        return { x: (1 - (y % 2)) * 1.5 * self.R + (3 * self.R * x) + self.R, y: ddy * y + ddy }
    };
    self.getTileCoords = getTileCoords;

    function getIdx(x, y) {
        return y * NX + x;
    }

    function getBabukOnTile(tileIdx, excludeBabu) {
        return babuk.filter(function (babu) {
            return babu.tileIdx == tileIdx && babu != excludeBabu;
        });
    }

    function removeBabu(babu) {
        var n = babuk.indexOf(babu);
        if (n==-1) return;
        
        selectedBabu = null;
        var idx = babu.tileIdx;
        babu.removeFromMap();
        //babuk.splice(n, 1);

        var tile = tiles[idx];
        tileLeft(idx, babu);

        if (tile.tileKind != TileKind.Init) { // tile was not removed.
            var group, gId = tile.group;
            $.each(groups, function (i, g) {
                if (g.id == gId) { group = g; return false }
            })

            var isolatedGroup = true, idxs = [];
            $.each(babuk, function (b, _babu) {
                if (_babu != babu && _babu.isOnMap() && tiles[_babu.tileIdx].group == gId) {
                    isolatedGroup = false;
                    return false;
                }
            })

            if (isolatedGroup) {
                if (group != null) {
                    idxs = group.tileIdxs.filter(function (idx, t) {
                        tiles[idx].isolated = true;
                        return tiles[idx].isRemovable()
                    });
                } else {
                    tiles.forEach(function (tile, i) {
                        tile.isolated = true;
                        if (tile.isRemovable()) idxs.push(i);
                    });
                    groups = [{ id: 1, tileIdxs: idxs }];
                }
                if (idxs.length) graphics.markIsolatedTiles(idxs);
            }
        }
        //selectedBabu = null;
        //graphics.removeBabuFromMap(babu);
        //babuk.splice(n, 1);
        nextMove();
        return true;
    }

    self.isSummer = function () { return nyar };
    self.isWinter = function () { return !nyar };

    self.drawMap = function () {
        for (var y = 0; y < self.NY; y++) {
            for (var x = 0; x < self.NX; x++) {
                var x0 = (1 - (y % 2)) * 1.5 * self.R + (3 * self.R * x) + self.R;
                var y0 = ddy * y + ddy;
                var i = y * self.NX + x;
                document.writeln(getH6area(x0, y0, i));
                h6(x0, y0, u);
            }
        }
    };

    self.drawBackGround = function () {
        for (var y = 0; y < self.NY; y++) {
            for (var x = 0; x < self.NX; x++) {
                var x0 = (1 - (y % 2)) * 1.5 * self.R + (3 * self.R * x);
                var y0 = ddy * y;
                var i = y * self.NX + x;
                document.write('<div class="base6" width="' + 2 * self.R + '" heigth="' + 2 * ddy + '" id="base' + i + '" ></div>');
                $('#base' + i).css({ 'left': x0 + 'px', 'top': y0 + 'px' });
            }
        }
    };

    self.adjustMapLayout = function () {
        hdn.map(function (idx) { self.setTile(idx, TileKind.Hidden) });
        nyariTabor.map(function (idx) { self.setTile(idx, TileKind.NyariTabor, 'NyariTabor') });
        teliTabor.map(function (idx) { self.setTile(idx, TileKind.TeliTabor, 'TeliTabor') });
        setSzomszedok();
    }

    self.init = function () {
        graphics.init(getTileCoords, getBabukOnTile);
        for (var i = 0; i < N; i++) {
            tiles[i] = new Tile();
        }
    };

    self.start = function () {
        boardElem = $('#board');
        return;

        self.adjustMapLayout();
        var jm = new JatekosManager(Localizer);
        var playersInfo = jm.getPlayersInfo(SzinKodok);
        var nPlayers = playersInfo.length;
        players = new Array(nPlayers);
        currentPhase = Phase.Nyar;
        playersInfo.forEach(function (info, i) {
            players[i] = new Jatekos(info.szin, info.nev);
            nyariTabor.map(function (idx, j) { babuk.push(new Babu(idx, players[i], i * self.maxBabuk + j)); });
        });
        self.shuffleTiles();
        tiles.forEach(function (tile, i) { if (tile.isBarlang()) barlang.push(i) });
        setCurrentPlayer(random(players.length));
    };

    function setCurrentPlayer(playerIdx) {
        currentPlayer = players[playerIdx];
        currentPlayerLepes = 0;
        lockedOnBabu = false;
        deselectCurrentBabu();
        selectedBabu = null;
        updateBoard();
    }

    self.setTile = function (idx, tileKind, kindStr) {
        //tiles[idx].tileKind = tileKind;
        graphics.drawTile(idx, tileKind, kindStr);
    };

    function allowActions() { processing = false; }

    function ongoingAction() {
        if (processing) {
            updateBoard( Localizer.processingAlready());
            return true;
        }
        processing = true;
        return false;
    }

    function processIfTeliTabor(idx) {
        if (currentPhase == Phase.Nyar && teliTabor.indexOf(idx) > -1) {
            var tile = tiles[idx];
            if (!tile.ladak || tile.ladak.indexOf(currentPlayer) == -1) {
                if (tile.ladak) tile.ladak.push(currentPlayer); else tile.ladak = [currentPlayer];
                graphics.drawLada(idx, currentPlayer.szin);
            }
        }
    }

    self.action = function (idx) {
        if (ongoingAction()) return;
        var result;
        if (selectedBabu) {
            if (idx > -1) {
                result = checkIfValidMove(idx);
                if (result === true || result == ResultCode.LockOnBabu) {
                    if (result == ResultCode.LockOnBabu) lockOnBabu();
                    var oldIdx = selectedBabu.tileIdx;
                    moveBabuTo(idx);
                    tileLeft(oldIdx, selectedBabu);
                    processIfTeliTabor(idx);
                    nextMove();
                } else {
                    message = Localizer.message(result);
                    updateBoard(message);
                }
            } else
                result = removeBabu(selectedBabu);
        } else if (idx > -1) {
            var tile = tiles[idx];
            if (tile.isRemovable()) {
                if (tile.isolated) {
                    removeIsolatedGroup(tile.group);
                    result = true;
                }
            }
        }
        if (!selectedBabu && result == null) {
            message = Localizer.doSomething(currentPlayer.nev);
            updateBoard(message);
        }
        allowActions();
    };

    function nextMove() {
        currentPlayerLepes++;
        if (currentPlayerLepes == 2) {
            switchToNextPlayer(true);
            return;
        }

        var jatekosHasBabu = playerHasBabu(currentPlayer);
        if (!jatekosHasBabu) {
            var nev = currentPlayer.nev;
            switchToNextPlayer(false);
            updateBoard(Localizer.playerHasNoBabu(nev));
        }

        //var sourceTile = tiles[selectedBabu.tileIdx];
        //var validTargetExists = sourceTile.isBarlang() && barlang.length > 1;
        //if (!validTargetExists)
        //    $.each(sourceTile.szomszedok, function (i, tileIdx) {
        //        if (checkIfValidTargetTile(tileIdx, sourceTile, tiles[tileIdx], true)) {
        //            validTargetExists = true;
        //            return false;
        //        }
        //    });

        //if (!validTargetExists) {
        //    updateBoard(Localizer.playerHasNoValidTargetTile(currentPlayer.nev));
        //}
    }

    function playerHasBabu(player) {
        var jatekosHasBabu;
        $.each(babuk, function(b, babu) {
            if (babu.jatekos == player && babu.isOnMap()) {
                jatekosHasBabu = true; return false;
            }
        });
        return jatekosHasBabu;
    }
    
    function switchToNextPlayer(dontRefreshBoard) {
        var n = players.indexOf(currentPlayer);
        var playerFound, newPlayer;
        while (!playerFound && newPlayer!=currentPlayer) {
            n++;
            if (n == players.length) n=0;
            newPlayer = players[n];
            playerFound = playerHasBabu(newPlayer);
        }

        if (newPlayer==currentPlayer) {
            endPhase();
        } else
            setCurrentPlayer(n);

        if (!dontRefreshBoard) updateBoard();
    }

    function endPhase() {
        if (currentPhase == Phase.Nyar)
            ittATel();
        else
            endGame();
    }

    function ittATel() {
        currentPhase = Phase.Tel;
        groups = [];
        babuk = [];
        graphics.initPhase();
        self.shuffleTiles();
        $.each(teliTabor, function (t, tileIdx) {
            if (tiles[tileIdx].ladak) {
                players.forEach(function(player) {
                    if (tile.ladak.indexOf(player)>=0) babuk.push(new Babu(tileIdx, player, player.id));
                });
                graphics.drawBabuk(babuk, tileIdx);
            }
        });
        var maxMamut = 0;
        babuk.forEach(function(babu) {
            if (!babu.jatekos.mamutPont) {
                babu.jatekos.mamutok.forEach(function (mamut) { jatekos.mamutPont += mamut.Pont});
                if (babu.jatekos.mamutPont > maxMamut) { currentPlayer = babu.jatekos }
            }
        })
        currentPlayerLepes = 0;
        updateBoard();
    }

    function tileLeft(idx, babu) {
        var babuk = getBabukOnTile(idx, babu);
        if (!babuk.length) {
            var tile = tiles[idx];
            var tileRemoved = self.removeTile(idx, tile);
            if (tileRemoved) babu.jatekos.processLeszedettTile(tile);
        }
    }

    function checkIfValidMove(idx, dontCheckSzomszeds) {
        if (!selectedBabu) return ResultCode.BabuNotSelected;
        if (idx == -1) return true;
        var sourceTile = tiles[selectedBabu.tileIdx];
        targetTile = tiles[idx];
        if (sourceTile == targetTile) return ResultCode.SameTile;
        return checkIfValidTargetTile(idx, sourceTile, targetTile);
    }

    function checkIfValidTargetTile(idx, sourceTile, targetTile, dontCheckSzomszeds) {
        if (!targetTile.isForBabu()) return ResultCode.InvalidTile;
        if (sourceTile.isBarlang() && targetTile.isBarlang()) return true;
        if (!dontCheckSzomszeds && targetTile.szomszedok.indexOf(selectedBabu.tileIdx) == -1) return ResultCode.NotSzomszedTile;
        if (targetTile == TileKind.Mamut && !currentPlayer.hasFegyver()) return ResultCode.NoWeapon;
        var destBabuk = getBabukOnTile(idx);
        if (destBabuk.length && !targetTile.allowsMoreBabus()) {
            if (!currentPlayer.currentLepes)
                return ResultCode.LockOnBabu;
            else
                return ResultCode.MoreBabusNotAllowed;
        }
        return true;
    }

    function lockOnBabu() {
        lockedOnBabu = true;
    }

    function moveBabuTo(idx) {
        graphics.moveBabu(selectedBabu, idx);
        graphics.selectBabu(selectedBabu);
    }

    function removeIsolatedGroup(groupId) {
        $.each(groups, function (g, group) {
            if (group.id == groupId) {
                $.each(group.tileIdxs, function (t, idx) { self.removeTile(idx) });
                graphics.unmarkIsolatedTiles(group.tileIdxs);
                return false;
            }
        });
    }

    self.removeTile = function (idx, tile) {
        if (!tile) tile = tiles[idx];
        if (!tile.isRemovable()) return false;

        $.each(tile.szomszedok, function (i, sz) {
            $.each(tiles[sz].szomszedok, function (j, szsz) {
                if (szsz == idx) {
                    tiles[sz].szomszedok.splice(j, 1); return false;
                }
            });
        });

        self.setTile(idx, TileKind.Init);

        if (tile.isolated) return;

        oldMaxGroupId = maxGroupId;
        $.each(tile.szomszedok, function (i, sz) {
            if (tiles[sz].group <= oldMaxGroupId)
                propagateGroup(tiles[sz], ++maxGroupId, oldMaxGroupId, sz);
        });

        groups = [];
        $.each(tiles, function (i, tile) {
            if (tile.isForBabu()) {
                var n = -1;
                $.each(groups, function (g, group) { if (group.id == tile.group) { n = g; return false } })
                if (n != -1)
                    groups[n].tileIdxs.push(i);
                else
                    groups.push({ id: tile.group, tileIdxs: [i] });
            }
        });

        $.each(babuk, function (b, babu) {
            if (babu.tileIdx < 0) return true;
            var gId = tiles[babu.tileIdx].group;
            $.each(groups, function (g, group) { if (group.id == gId) { group.hasBabu = true; return false } });
        });

        var isolatedGroups = groups.filter(function (group, g) { return !group.hasBabu });

        if (isolatedGroups.length) {
            var idxs = [];
            $.each(isolatedGroups, function (g, group) {
                idxs = idxs.concat(group.tileIdxs.filter(function (idx, t) {
                    tiles[idx].isolated = true;
                    return tiles[idx].isRemovable()
                }))
            });
            if (idxs.length) graphics.markIsolatedTiles(idxs);
        }
        return true;
    };

    function propagateGroup(tile, newGroupId, oldMaxGroupId, idx) {
        if (tile.group <= oldMaxGroupId && tile.group != newGroupId) {
            tile.group = newGroupId;
            //$('#base' + idx).text(newGroupId + ' ' + idx);
            $.each(tile.szomszedok, function (i, sz) { propagateGroup(tiles[sz], newGroupId, oldMaxGroupId, sz) });
            if (tile.isBarlang()) $.each(barlang, function (i, sz) { propagateGroup(tiles[sz], newGroupId, oldMaxGroupId, sz) });
        }
    }

    function setSzomszedok() {
        tiles.forEach(function (tile, tileIdx) {
            if (tile.tileKind != TileKind.Hidden) {
                var y = Math.floor(tileIdx / app.NX), x = tileIdx % app.NX;
                var parosy = (y % 2) == 0;
                var szomszedok = [];
                if (y > 1) szomszedok.push((y - 2) * app.NX + x);
                if (y > 0 && x + parosy < app.NX) szomszedok.push((y - 1) * app.NX + x + parosy);
                if (y < app.NY - 1 && x + parosy < app.NX) szomszedok.push((y + 1) * app.NX + x + parosy);
                if (y < app.NY - 2) szomszedok.push((y + 2) * app.NX + x);
                var newX = x - 1 + parosy;
                if (newX >= 0 && newX < app.NX) {
                    if (y < app.NY - 1) szomszedok.push((y + 1) * app.NX + newX);
                    if (y > 0) szomszedok.push((y - 1) * app.NX + newX);
                }
                tile.szomszedok = szomszedok.filter(function (idx) { return tiles[idx].tileKind != TileKind.Hidden });
            }
        })
    }

    self.getSzomszedok = function (idx) { return tiles[idx].szomszedok };

    self.selectBabu = function (b) {
        if (ongoingAction()) return;
        if (currentPlayer) {
            var babu = babuk[b];
            if (babu.jatekos != currentPlayer) {
                updateBoard(Localizer.foreignBabu(currentPlayer.nev, babu.jatekos.nev));
            } else if (lockedOnBabu && babu!=selectedBabu)
                updateBoard(Localizer.notTheLockedBabu(currentPlayer.nev));
            else {
                deselectCurrentBabu();
                selectedBabu = babu;
                graphics.selectBabu(selectedBabu);
            } 
        }
        allowActions();
    };

    function deselectCurrentBabu() {
        if (!selectedBabu) return;
        graphics.deselectBabu(selectedBabu);
    }

    self.init();
    return self;
}

var graphics = new Graphics();
var app = new App();
app.init();

$(function () {
    var msgBoard = $('#msgBoard');
    graphics.babuTemplate = getClone('#tmp_babu');
    graphics.ladaTemplate = getClone('#tmp_lada');

    app.start();

    //$.connection.hub.url = "/signalr";

    var partyId = getQueryParam('partyId');
    app.partyId = partyId;
    mv = $.connection.mVHub;

    $.connection.hub.qs = "partyId=" + partyId;

    mv.client.something = function (a, b, c, d) {
        debugger;
    };

    $.connection.hub.start().done(function () {
        //mv.server.getStatus(partyId);
    });

    mv.client.playerJoined = function (nick, id) {
        write({ bold: true, msg: Localizer.playerJoined(nick) });
        app.setPlayerStatus(nick, id, true);
        app.updateBoard(app.mvParty);
    };

    mv.client.playerLeft = function (nick, id) {
        write({ bold: true, msg: Localizer.playerLeft(nick) });
        app.setPlayerStatus(nick, id, false);
        app.updateBoard(app.mvParty);
    };

    mv.client.setStatus = function (mvInfo, playerId) {
        if (!playerId) playerId = app.mvParty.jatekos.Id;
        app.mvParty = $.parseJSON(mvInfo);
        if (playerId) {
            $.each(app.mvParty.players, function (i, p) {
                if (p.Id == playerId) {
                    app.mvParty.jatekos = p;
                    return false;
                }
            });
        }
        if (app.mvParty.tiles)
            $.each(app.mvParty.tiles, function (t, tile) { app.setTile(t, tile.tileKind) });
        app.updateBoard(app.mvParty);
        $('#ddSzin option').each(function (i, x) {
            $(this).addClass(graphics.getSzinClass(x.value));
        });
    };


    function write(content) {
        var elem = $('<div></div>');
        if (content.bold) elem.css('font-weight', 'bold');
        elem.text(content.msg);
        msgBoard.append(elem);
    }

    $('#wrapper').on('click', 'area', function (e) {
        var tileIdx = $(this).data('tile-id');
        app.action(tileIdx);
    });

    $('#players').on('click', '[data-babu]', function (e) {
        var a = $(this).data('babu');
        app.selectBabu(a);
    });

    $('#board').on('click', '[data-remove-babu]', function (e) {
        app.action(-1);
    });

});