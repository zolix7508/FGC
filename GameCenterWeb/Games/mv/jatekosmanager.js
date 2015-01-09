//var JatekosManager = function(localizer) {
//	var self = this;
//	var nPlayers;
	
//	function GetSzinString(name, szinkodok) {
//		var s='';
//		szinkodok.some( function (kod, i) {
//			if (kod) s+='{0} - {1}, '.format(kod, localizer.playerColorName(kod));
//		});
//		s = s.substr(0, s.length-2);
//		return '{0} színe? ({1})'.format(name, s); 
//	}
	
//	function getNPlayers() {
//		while (true) {
//			var n = prompt(localizer.messages.howManyUsers);
//			if (n!=NaN && (n.length==1) && n>=2 && n<=4) return n;
//		}
//	};
	
//	function getPlayerInfo(i, szinkodok, playersInfo) {
//		var un, szinkod;
//		while (!un) {
//			un = prompt(localizer.askUserName(i));
//			if (un) un=un.trim();
//			if (playersInfo.some( function(x,i) {
//				return x.nev == un;
//			})) {
//				alert(localizer.duplicateUser(un));
//				un=null;
//			}
//		}
//		if (szinkodok.length>1) {
//		while (!szinkod) {
//			szinkod = prompt(GetSzinString(un, szinkodok));
//			if (szinkod) szinkod = szinkod.trim();
//			var idx = szinkodok.indexOf(szinkod);
//			if (idx < 0) szinkod = null;
//		}
//		} else if (szinkodok.length)
//			szinkod = szinkodok[0];
			
//		return { nev: un, szin: szinkod };
//	};
	
//	self.getPlayersInfo = function(szinkodok) {
//		var n = getNPlayers();	
//		var playersInfo = [], jatekosInfo;

//		for (var i=1; i<n; i++) {
//			jatekosInfo = getPlayerInfo(i, szinkodok, playersInfo);
//			playersInfo.push(jatekosInfo);
//			var idx = szinkodok.indexOf(jatekosInfo.szin);
//			if (idx>-1) szinkodok.splice(idx, 1)
//		}
//		jatekosInfo = getPlayerInfo(n, szinkodok, playersInfo);
//		playersInfo.push(jatekosInfo);
//		return playersInfo;
//	};
	
//}
