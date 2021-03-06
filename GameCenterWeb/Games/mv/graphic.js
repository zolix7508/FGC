function Graphics() {
    var self = this;
    var babuR = 37;
    var babuRp2 = babuR / 2;
    var babuKorR = 23;
    var ladaKorR = 45, ladaRp2 = ladaKorR / 2;
    var getTileCoords;
    var getBabukOnTile;
    var isTel;
    var host;
    self.babuTemplate, self.ladaTemplate;
	
    self.init = function (_getTileCoords, _getBabukOnTile, _isTel) {
        getTileCoords = _getTileCoords;
        getBabukOnTile = _getBabukOnTile;
        isTel = _isTel;
    };
	
    self.getSzinClass = function(szinkod) {
        switch (szinkod) {
            case '0': return 'szin_0';
            case '1': return 'szin_p';
            case '2': return 'szin_k';
            case '3': return 'szin_z';
            case '4': return 'szin_s';
        }
    }

	self.getSzinkod = function(szinkod) {
	    switch (szinkod) {
	        case 1: return 'p';
	        case 2: return 'k';
	        case 3: return 'z';
	        case 4: return 's';
	    }
	}

	self.drawBabuk = function (babuk, tileIdx, players) {
	    var jatekosok = [];
	    var babuk = babuk.filter(function (babu) { return babu.tileIdx == tileIdx });
	    babuk.forEach(function (babu, i) {
	        var jatekos = players[babu.playerIdx];
	        var n = jatekosok.indexOf(jatekos);
	        if (n == -1) {
	            $.merge(jatekosok, [jatekos, { babuk: [babu] }]);
	        } else
	            $.merge(jatekosok[n + 1].babuk, [babu]);
	    });

	    var u = 0, n = _2PI / jatekosok.length;
	    var co = getTileCoords(tileIdx);
	    var x0 = co.x - babuRp2, y0 = co.y - babuRp2;
	    for (i = 0; i < jatekosok.length; i += 2) {
	        var x = x0 + babuKorR * Math.cos(u);
	        var y = y0 + babuKorR * Math.sin(u);
	        var info = jatekosok[i + 1];
	        var cls = 'babu_' + self.getSzinkod(jatekosok[i].Szinkod);
	        info.babuk.forEach(function (babu) {
	            var elem = $('[data-babu="' + babu.id + '"]', host);
	            var isNew = !elem.length;
	            if (isNew) elem = self.babuTemplate.clone().addClass(cls).attr('data-babu', babu.id);
	            elem.css({ left: x, top: y }).find('.babu-szam').text(info.babuk.length > 1 ? info.babuk.length : '');
	            if (isNew) host.append(elem);
	        });
	        u += Math.PI / 2;
	    }
	}

	self.removeBabu = function (babuk, tileIdx, players) {
	    var babuk = babuk.filter(function (babu) { return babu.tileIdx == tileIdx });
	    self.drawBabuk(babuk, tileIdx, players);
	};
	
	self.putBabu = function (babu, idx, players) {
	    var babuk = getBabukOnTile(idx);
	    babu.tileIdx = idx;
	    var n = babuk.indexOf(babu);
	    if (n==-1) babuk.push(babu);
	    self.drawBabuk(babuk, idx, players);
	};

	self.removeBabuFromMap = function (babuId) {
	    $('[data-babu="' + babuId + '"]', host).remove();
	}

	self.initPhase = function () {
	    $('[data-babu],[data-lada]', host).remove();
	};

	self.drawTile = function (idx, tileKind, kindStr, isolated, keepRotation) {
	    if (!kindStr) kindStr = TileKind.tileKindNames[tileKind - ((tileKind > 6) ? 1 : 0)];
	    var elem = $('#base' + idx);
	    switch (tileKind) {
	        case TileKind.Hidden:
	            elem.hide();
	            return;
	        case TileKind.Init:
	            elem.attr('class', 'base6');
	            break;
	    };
	    elem.addClass('tile-' + (isTel() ? 't' : '') + kindStr);
	    if (!keepRotation) {
	        var rdeg = 'rotate({0}deg)'.format(60 * Math.floor(Math.random() * 6));
	        elem.css({ '-webkit-transform': rdeg, '-moz-transform': rdeg, '-ms-transform': rdeg, '-o-transform': rdeg, 'transform': rdeg });
	    }

	    if (isolated) markIsolatedTile(idx, tileKind != TileKind.Init);
	};

	self.drawLada = function (tileIdx, szin) {
	    var u = Math.PI / 2 * (szin - 1);
	    var co = getTileCoords(tileIdx);
	    var x = co.x - ladaRp2 + ladaKorR * Math.cos(u);
	    var y = co.y - ladaRp2 + ladaKorR * Math.sin(u);
	    var cls = 'lada_' + self.getSzinkod(szin);
	    var elem = self.ladaTemplate.clone().addClass(cls).css({ left: x, top: y });
	    host.append(elem);
	}

	self.markIsolatedTiles = function (tileIdxs) {
	    self.markIsolated(tileIdxs, true);
	};

	self.unmarkIsolatedTiles = function (tileIdxs) {
	    self.markIsolated(tileIdxs, false);
	};

	self.selectBabu = function (babu) {
	    var p = $('[data-babu=' + babu.id + ']:first').position();
	    $('#selectedBabu').css({left: p.left, top: p.top, display:'inherit'});
	};

	self.deselectBabu = function (babu) {
	    $('#selectedBabu').hide();
	};

	self.markIsolated = function(idxs, mark) {
	    var s = '';
	    $.each(idxs, function (i, x) { s += ',#base' + x; });
	    if (s) {
	        s = s.substr(1);
	        s = $(s);
	        if (mark) s.fadeTo(500, 0.5, function () { s.addClass('isolatedTile') });
	        else (s.removeClass('isolatedTile').fadeTo(500, 1));
	    }
	}

	function markIsolatedTile(idx, mark) {
	    var elem = $('#base' + idx);
	    if (mark) elem.fadeTo(500, 0.5, function () { elem.addClass('isolatedTile') });
	    else (elem.removeClass('isolatedTile').fadeTo(500, 1));
	}

	$(function () {
	    host = $('#players');
	});
	return self;
};