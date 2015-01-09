
var ctx, mv, msgBoard;

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

function App() {
    var self = this;

    self.szervizOn;

    self.isRandom = function (tile) { return [TileKind.Hidden, TileKind.NyariTabor, TileKind.TeliTabor].indexOf(tile.tileKind) < 0 };
    self.isRemovable = function (tile) { return self.isRandom(tile) & [TileKind.Barlang, TileKind.Ut/*, TileKind.Init*/].indexOf(tile.tileKind) < 0 };

    self.partyId;
    self.mvParty;
    //self.jatekos = {};


    self.R = 50;
    self.NX = 6;
    self.NY = 21;

    var PII = 2 * Math.PI - 0.001;
    var u = Math.PI / 3;
    //var ddx = self.R * Math.cos(u);
    var ddy = self.R * Math.sin(u);
    //self.ddy = ddy;

    //var nyar = true;
    //var babuk = [];

    //var hdn = [4, 5, 114, 120, 123, 125];
    //var nyariTabor = [19, 84, 35, 112];
    //var teliTabor = [56, 98, 21, 70];

    //var nyariKovek = { Bogyo: 30, Ut: 19, Gyoker: 18, Fuszer: 5, Korso: 10, Nyaklanc: 10, Fegyver: 12, Mamut: 3, Barlang: 5 };
    //var teliKovek = { Bogyo: 30, Gyoker: 18, Fuszer: 5, Irha: 10, Koponya: 10, Fegyver: 3, Mamut: 12 };
    //var barlang = [];

    //var selectedBabu;
    var currentPlayer;
    //var currentPhase, currentPlayerLepes;
    //self.currentPlayer = currentPlayer;

    var boardElem, processing;
    var gep = Localizer.gepNick;

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

    self.szerviz = function () {
        if (!self.szervizOn) return;
        $.each(self.mvParty.tiles, function (t, tile) {
            $('#base' + t).text(tile.group + ' ' + t);
        })
    };

    self.updateBoard = function updateBoard(info) {
        var sc = angular.element(boardElem).scope();
        if (sc) {
            sc.$apply(function () {
                sc.exData(info);
            });
        }
    };

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

    //self.shuffleTiles = function () {
    //    var arr = [];
    //    tiles.forEach(function (tile, i) {
    //        tile.isolated = false;
    //        if (tile.isRandom()) arr.push(i);
    //    });

    //    var kovek = currentPhase == Phase.Nyar ? nyariKovek : teliKovek;
    //    for (var kind in kovek) {
    //        var tileKindId = eval('TileKind.' + kind);
    //        var b = kovek[kind];
    //        while (b-- && arr.length) {
    //            var i = Math.floor(arr.length * Math.random());
    //            self.setTile(arr[i], tileKindId, kind);
    //            arr.splice(i, 1);
    //        }
    //    }
    //};

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

    function getBabukOnTile(tileIdx, excludeBabu) {
        return self.mvParty.babuk.filter(function (babu) {
            return babu.tileIdx == tileIdx && babu != excludeBabu;
        });
    }

    self.drawMap = function () {
        for (var y = 0; y < self.NY; y++) {
            for (var x = 0; x < self.NX; x++) {
                var i = y * self.NX + x;
                if ([4,5,114,120,123,125].indexOf(i) > -1) continue;
                var x0 = (1 - (y % 2)) * 1.5 * self.R + (3 * self.R * x) + self.R;
                var y0 = ddy * y + ddy;
                
                document.writeln(getH6area(x0, y0, i));
                h6(x0, y0, u);
            }
        }
    };

    self.drawBackGround = function () {
        for (var y = 0; y < self.NY; y++) {
            for (var x = 0; x < self.NX; x++) {
                var i = y * self.NX + x;
                if ([4, 5, 114, 120, 123, 125].indexOf(i) > -1) continue;
                var x0 = 1+(1 - (y % 2)) * 1.5 * self.R + (3 * self.R * x);
                var y0 = 1+ddy * y;
                document.write('<div class="base6" width="' + 2 * self.R + '" heigth="' + 2 * ddy + '" id="base' + i + '" ></div>');
                $('#base' + i).css({ 'left': x0 + 'px', 'top': y0 + 'px' });
            }
        }
    };

    //self.adjustMapLayout = function () {
    //    hdn.map(function (idx) { self.setTile(idx, TileKind.Hidden) });
    //    nyariTabor.map(function (idx) { self.setTile(idx, TileKind.NyariTabor, 'NyariTabor') });
    //    teliTabor.map(function (idx) { self.setTile(idx, TileKind.TeliTabor, 'TeliTabor') });
    //    setSzomszedok();
    //}

    self.init = function () {
        graphics.init(getTileCoords, getBabukOnTile);
    };

    self.start = function () {
        boardElem = $('#board');
    };

    //function setCurrentPlayer(playerIdx) {
    //    currentPlayer = players[playerIdx];
    //    currentPlayerLepes = 0;
    //    lockedOnBabu = false;
    //    deselectCurrentBabu();
    //    selectedBabu = null;
    //    updateBoard();
    //}

    self._setCurrentPlayer = function (player) {
        currentPlayer = player;
    }

    self.setTile = function (idx, tileKind, kindStr, isolated, keepRotation) {
        graphics.drawTile(idx, tileKind, kindStr, isolated, keepRotation);
    };

    self.sendMessage = function(sender, message) {
        if (message) {
            formatChatMsg(sender, message);
            mv.server.sendMessage(message);
        }
    }

    function formatChatMsg(sender, message) {
        if (sender)
            return '{0}: {1}'.format(sender, message);
        return message;
    }

    function allowActions() { processing = false; }

    function ongoingAction() {
        if (processing) {
            self.sendMessage( gep, Localizer.processingAlready());
            return true;
        }
        processing = true;
        return false;
    }

    //function processIfTeliTabor(idx) {
    //    if (currentPhase == Phase.Nyar && teliTabor.indexOf(idx) > -1) {
    //        var tile = tiles[idx];
    //        if (!tile.ladak || tile.ladak.indexOf(currentPlayer) == -1) {
    //            if (tile.ladak) tile.ladak.push(currentPlayer); else tile.ladak = [currentPlayer];
    //            graphics.drawLada(idx, currentPlayer.szin);
    //        }
    //    }
    //}

    self.action = function (idx) {
        if (currentPlayer) {
            if (currentPlayer.Id == self.mvParty.jatekos.Id) {
                if (ongoingAction()) return;
                mv.server.action(idx);
                allowActions();
            } else
                write({ msg: Localizer.notYourTurn() });
        }
    };

    //function nextMove() {
    //    currentPlayerLepes++;
    //    if (currentPlayerLepes == 2) {
    //        switchToNextPlayer(true);
    //        return;
    //    }

    //    var jatekosHasBabu = playerHasBabu(currentPlayer);
    //    if (!jatekosHasBabu) {
    //        var nev = currentPlayer.nev;
    //        switchToNextPlayer(false);
    //        updateBoard(Localizer.playerHasNoBabu(nev));
    //    }

    //    //var sourceTile = tiles[selectedBabu.tileIdx];
    //    //var validTargetExists = sourceTile.isBarlang() && barlang.length > 1;
    //    //if (!validTargetExists)
    //    //    $.each(sourceTile.szomszedok, function (i, tileIdx) {
    //    //        if (checkIfValidTargetTile(tileIdx, sourceTile, tiles[tileIdx], true)) {
    //    //            validTargetExists = true;
    //    //            return false;
    //    //        }
    //    //    });

    //    //if (!validTargetExists) {
    //    //    updateBoard(Localizer.playerHasNoValidTargetTile(currentPlayer.nev));
    //    //}
    //}

    //function playerHasBabu(player) {
    //    var jatekosHasBabu;
    //    $.each(babuk, function(b, babu) {
    //        if (babu.jatekos == player && babu.isOnMap()) {
    //            jatekosHasBabu = true; return false;
    //        }
    //    });
    //    return jatekosHasBabu;
    //}
    
    //function switchToNextPlayer(dontRefreshBoard) {
    //    var n = players.indexOf(currentPlayer);
    //    var playerFound, newPlayer;
    //    while (!playerFound && newPlayer!=currentPlayer) {
    //        n++;
    //        if (n == players.length) n=0;
    //        newPlayer = players[n];
    //        playerFound = playerHasBabu(newPlayer);
    //    }

    //    if (newPlayer==currentPlayer) {
    //        endPhase();
    //    } else
    //        setCurrentPlayer(n);

    //    if (!dontRefreshBoard) updateBoard();
    //}

    //function endPhase() {
    //    if (currentPhase == Phase.Nyar)
    //        ittATel();
    //    else
    //        endGame();
    //}

    //function ittATel() {
    //    currentPhase = Phase.Tel;
    //    groups = [];
    //    babuk = [];
    //    graphics.initPhase();
    //    self.shuffleTiles();
    //    $.each(teliTabor, function (t, tileIdx) {
    //        if (tiles[tileIdx].ladak) {
    //            players.forEach(function(player) {
    //                if (tile.ladak.indexOf(player)>=0) babuk.push(new Babu(tileIdx, player, player.id));
    //            });
    //            graphics.drawBabuk(babuk, tileIdx);
    //        }
    //    });
    //    var maxMamut = 0;
    //    babuk.forEach(function(babu) {
    //        if (!babu.jatekos.mamutPont) {
    //            babu.jatekos.mamutok.forEach(function (mamut) { jatekos.mamutPont += mamut.Pont});
    //            if (babu.jatekos.mamutPont > maxMamut) { currentPlayer = babu.jatekos }
    //        }
    //    })
    //    currentPlayerLepes = 0;
    //    updateBoard();
    //}

    //function tileLeft(idx, babu) {
    //    var babuk = getBabukOnTile(idx, babu);
    //    if (!babuk.length) {
    //        var tile = tiles[idx];
    //        var tileRemoved = self.removeTile(idx, tile);
    //        if (tileRemoved) babu.jatekos.processLeszedettTile(tile);
    //    }
    //}

    //function checkIfValidMove(idx, dontCheckSzomszeds) {
    //    if (!selectedBabu) return ResultCode.BabuNotSelected;
    //    if (idx == -1) return true;
    //    var sourceTile = tiles[selectedBabu.tileIdx];
    //    targetTile = tiles[idx];
    //    if (sourceTile == targetTile) return ResultCode.SameTile;
    //    return checkIfValidTargetTile(idx, sourceTile, targetTile);
    //}

    //function checkIfValidTargetTile(idx, sourceTile, targetTile, dontCheckSzomszeds) {
    //    if (!targetTile.isForBabu()) return ResultCode.InvalidTile;
    //    if (sourceTile.isBarlang() && targetTile.isBarlang()) return true;
    //    if (!dontCheckSzomszeds && targetTile.szomszedok.indexOf(selectedBabu.tileIdx) == -1) return ResultCode.NotSzomszedTile;
    //    if (targetTile == TileKind.Mamut && !currentPlayer.hasFegyver()) return ResultCode.NoWeapon;
    //    var destBabuk = getBabukOnTile(idx);
    //    if (destBabuk.length && !targetTile.allowsMoreBabus()) {
    //        if (!currentPlayer.currentLepes)
    //            return ResultCode.LockOnBabu;
    //        else
    //            return ResultCode.MoreBabusNotAllowed;
    //    }
    //    return true;
    //}

    //function lockOnBabu() {
    //    lockedOnBabu = true;
    //}

    //function removeIsolatedGroup(groupId) {
    //    $.each(groups, function (g, group) {
    //        if (group.id == groupId) {
    //            $.each(group.tileIdxs, function (t, idx) { self.removeTile(idx) });
    //            graphics.unmarkIsolatedTiles(group.tileIdxs);
    //            return false;
    //        }
    //    });
    //}

    //self.removeTile = function (idx, tile) {
    //    if (!tile) tile = tiles[idx];
    //    if (!tile.isRemovable()) return false;

    //    $.each(tile.szomszedok, function (i, sz) {
    //        $.each(tiles[sz].szomszedok, function (j, szsz) {
    //            if (szsz == idx) {
    //                tiles[sz].szomszedok.splice(j, 1); return false;
    //            }
    //        });
    //    });

    //    self.setTile(idx, TileKind.Init);

    //    if (tile.isolated) return;

    //    oldMaxGroupId = maxGroupId;
    //    $.each(tile.szomszedok, function (i, sz) {
    //        if (tiles[sz].group <= oldMaxGroupId)
    //            propagateGroup(tiles[sz], ++maxGroupId, oldMaxGroupId, sz);
    //    });

    //    groups = [];
    //    $.each(tiles, function (i, tile) {
    //        if (tile.isForBabu()) {
    //            var n = -1;
    //            $.each(groups, function (g, group) { if (group.id == tile.group) { n = g; return false } })
    //            if (n != -1)
    //                groups[n].tileIdxs.push(i);
    //            else
    //                groups.push({ id: tile.group, tileIdxs: [i] });
    //        }
    //    });

    //    $.each(babuk, function (b, babu) {
    //        if (babu.tileIdx < 0) return true;
    //        var gId = tiles[babu.tileIdx].group;
    //        $.each(groups, function (g, group) { if (group.id == gId) { group.hasBabu = true; return false } });
    //    });

    //    var isolatedGroups = groups.filter(function (group, g) { return !group.hasBabu });

    //    if (isolatedGroups.length) {
    //        var idxs = [];
    //        $.each(isolatedGroups, function (g, group) {
    //            idxs = idxs.concat(group.tileIdxs.filter(function (idx, t) {
    //                tiles[idx].isolated = true;
    //                return tiles[idx].isRemovable()
    //            }))
    //        });
    //        if (idxs.length) graphics.markIsolatedTiles(idxs);
    //    }
    //    return true;
    //};

    self.selectBabu = function (b) {
        if (currentPlayer) {
            if (currentPlayer.Id == self.mvParty.jatekos.Id) {
                if (ongoingAction()) return;
                mv.server.babuClicked(b);
                allowActions();
            } else
                write({ msg: Localizer.notYourTurn() });
        }
    };

    self.processBabu = function(babuk, id) {
        if (id >= 0) {
            var babu;
            $.each(babuk, function(i, bb) { if (bb.id==id) babu = bb });
            graphics.selectBabu(babu);
        } else 
            graphics.deselectBabu();
    }
    self.init();
    return self;
}

var graphics = new Graphics();
var app = new App();
app.init();

function write(content) {
    var elem = $('<div></div>');
    if (content.bold) elem.css('font-weight', 'bold');
    elem.html(content.msg);
    msgBoard.append(elem).scrollTop(msgBoard[0].scrollHeight - msgBoard[0].clientHeight);
}

$(function () {
    msgBoard = $('#msgBoard');
    graphics.babuTemplate = getClone('#tmp_babu');
    graphics.ladaTemplate = getClone('#tmp_lada');
    graphics.deselectBabu();

    function setCurrentPlayer() {
        if (app.mvParty.players && app.mvParty.CurrentPlayerIdx >= 0)
            app._setCurrentPlayer(app.mvParty.players[app.mvParty.CurrentPlayerIdx]);
        else
            app._setCurrentPlayer(null);
    }

    app.start();

    //$.connection.hub.url = "/signalr";

    var partyId = getQueryParam('partyId');
    app.partyId = partyId;
    mv = $.connection.mVHub;

    $.connection.hub.qs = "partyId=" + partyId;

    $.connection.hub.error(function (error) {
        var eelem = $('#error-text');
        eelem.html(eelem.html() + '<br/>' +  error);
        $('#error').show();
    });

    //contosoChatHubProxy.newContosoChatMessage(userName, message)
    //.fail(function (error) {
    //    console.log('newContosoChatMessage error: ' + error)
    //});

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

    function setStatus(mvInfo, playerId) {
        if (!playerId) playerId = app.mvParty.jatekos.Id;
        var prevTiles;
        if (app.mvParty) prevTiles = app.mvParty.tiles;
        app.mvParty = isString(mvInfo) ? $.parseJSON(mvInfo) : mvInfo;
        if (playerId) {
            $.each(app.mvParty.players, function (i, p) {
                if (p.Id == playerId) {
                    app.mvParty.jatekos = p;
                }
                $.each(p.ladak, function (l, lada) {
                    graphics.drawLada(lada, p.Szinkod);
                });
            });
        }
        if (app.mvParty.tiles)
            $.each(app.mvParty.tiles, function (t, tile) {
                app.setTile(t, tile.tileKind, null, tile.isolated && app.isRemovable(tile), prevTiles && tile.tileKind == prevTiles[t].tileKind);
            });

        if (app.mvParty.babuk)
            $.each(app.mvParty.babuk, function (b, babu) { if (babu.tileIdx >= 0) graphics.putBabu(babu, babu.tileIdx, app.mvParty.players) });

        setCurrentPlayer();

        app.processBabu(app.mvParty.babuk, app.mvParty.selectedBabuId);
        app.updateBoard(app.mvParty);

        $('#ddSzin option').each(function (i, x) {
            $(this).addClass(graphics.getSzinClass(x.value));
        });

        app.szerviz();
    }

    mv.client.setStatus = function (mvInfo, playerId) {
        setStatus(mvInfo, playerId);
    };

    mv.client.msg = function (msg) {
        var prms = JSON.stringify(msg.slice(1));
        if (prms.length < 2) return;
        prms = prms.substr(1, prms.length - 2);
        var m = 'Localizer.{0}({1})'.format(msg[0], prms);
        msg = eval(m);
        write({ msg: msg });
    }

    mv.client.processBabu = function (id) {
        app.processBabu(app.mvParty.babuk, id);
    }

    mv.client.removeBabu = function (babuId) {
        graphics.removeBabuFromMap(babuId);
        app.szerviz();
    }

    mv.client.drawBabuk = function (babuk, tileIdx, players) {
        graphics.drawBabuk(babuk, tileIdx, players);
    }

    mv.client.tileRemoved = function (idx, isolatedTiles) {
        if (idx >= 0) {
            graphics.drawTile(idx, TileKind.Init);
        }
        if (isolatedTiles && isolatedTiles.length) graphics.markIsolatedTiles(isolatedTiles);
        app.szerviz();
    }

    mv.client.removeIsolated = function (idx, isolatedTiles) {
        if (isolatedTiles) {
            graphics.unmarkIsolatedTiles(isolatedTiles);
            $.each(isolatedTiles, function (t, idx) {
                graphics.drawTile(idx, TileKind.Init);
            });
        }
    }

    mv.client.updateCurrentPlayer = function (players, currentPlayerIdx, currentPlayerLepes) {
        app.mvParty.players = players;
        if (app.mvParty.CurrentPlayerIdx != currentPlayerIdx) graphics.deselectBabu();
        app.mvParty.CurrentPlayerIdx = currentPlayerIdx;
        app.mvParty.CurrentPlayerLepes = currentPlayerLepes;
        app.updateBoard(app.mvParty);
        setCurrentPlayer();
    }

    mv.client.ladaDeployed = function (idx, szinkod) {
        graphics.drawLada(idx, szinkod);
    }

    mv.client.winterStart = function (mvInfo) {
        graphics.initPhase();
        setStatus(mvInfo, null);
        write({ bold: true, msg: Localizer.winterStart() });
    }

    mv.client.messageReceived = function (message) {
        write(message);
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