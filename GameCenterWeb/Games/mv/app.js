
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

    self.R = 50;
    self.NX = 6;
    self.NY = 21;

    var PII = 2 * Math.PI - 0.001;
    var u = Math.PI / 3;
    var ddy = self.R * Math.sin(u);
    
    var currentPlayer;
    
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

    function isTel() { return self.mvParty.phase == Phase.Tel }

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

    self.init = function () {
        graphics.init(getTileCoords, getBabukOnTile, isTel);
    };

    self.start = function () {
        boardElem = $('#board');
    };

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
                var mo = [];
                p.mamutok.sort();
                p.mamutok.forEach(function (m) {
                    var n = -1;
                    $.each(mo, function (nn, mm) { if (mm.m == m) { n = nn; return false } });
                    if (n == -1) mo.push({ m: m, count: 1 });
                    else mo[n].count++;
                });
                p.mLapkak = mo;
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

    mv.client.allas = function (results) {
        var sc = angular.element('#results').scope();
        if (sc) {
            sc.$apply(function () {
                sc.playerResults = results;
            });
        }
    }

    mv.client.endGame = function (playerResults) {
        mv.client.allas(playerResults);
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

    $('#btnAllas').on('click', function (e) {
        mv.server.getAllas();
    });

});