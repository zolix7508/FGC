var Localizer = {
	messages: {
		howManyUsers : 'Hány játékos lesz? (2-4)',
		invalidUserName: 'Érvénytelen játékosnév!',
		nextPlayerMsg: '{0} következik!',
		points: 'p.',
        um: 'drb'
	},
	
    gepNick: 'Gép',
	askUserName: function(i) { return '{0}. játékos neve?'.format(i);},
	duplicateUser: function (name) { return '\'{0}\' nevu játékosunk már van!'.format(name); },
	nextPlayer: function (name, lepes) { return '{0} következik! ({1}. lépés)'.format(name, lepes); },
	doSomething: function (name) { return '{0}, ki kéne választani előbb egy bábut vagy ilyesmi.'.format(name) },
	processingAlready: function () { return 'Dolgozom, várjatok egy kicsit...' },
	foreignBabu: function (currentPlayerName, foreignPlayerName) { return '{0}, ne {1} bábujával!'.format(currentPlayerName, foreignPlayerName) },
	playerHasNoBabu: function (name) { return '{0}, nincs több bábud a pályán :('.format(name) },
	notTheLockedBabu: function (name) { return '{0}, az előbbi bábuval kell most lépned újra. Lépés befejezésekor csak Barlang vagy Táborhely lapkán maradhat egynél több bábu!'.format(name) },
	playerJoined: function (nick) { return '*** {0} belépett.'.format(nick) },
	playerLeft: function (nick) { return '*** {0} kilépett.'.format(nick) },
	connStatus: function (online) { return online ? 'Itt van.' : 'Nincs itt.' },
	notYourTurn: function () { return 'Most nem rajtad van a sor. Hát ez van.' },
	//notYourTurn: function (nick) { return '{0}, most nem rajtad van a sor. Hát ez van.'.format(nick) },
	//notYourTurnLocal: function () { return 'Most nem rajtad van a sor. Hát ez van.' },
	notLoggedIn: function () { return 'Van egy kis gond. Újra be kéne jelentkezzél. <a href="/Account/Login">Klikk ide!</a>' },
	notYourParty: function () { return 'Van egy kis gond. Nem ehhez a parthoz vagy bejelentkezve. Ki/be jelentkezéshez <a href="/Account/Login">klikk ide!</a>' },
	winterStart: function () { return 'Vége van a nyárnak...<br/>Hűvős szelek járnak<br/>Nagy bánata van a mamutcsordának!'; },

	playerColorName: function (kod) {
			switch (kod) {
				case '1': return 'Piros';
			    case '2': return 'Kék'; 
			    case '3': return 'Zöld';
				case '4': return 'Sárga';
			}
	},

	message: function (code, nick) {
	    var msg = this.rawMessage(code);
	    return '{0}, {1}'.format(nick, msg);
	},

	rawMessage: function (code) {
	    switch (code) {
	        case ResultCode.BabuNotSelected: return 'és melyik bábut mozgassam? Válassz ki egy bábut elébb, légyszi.';
	        case ResultCode.InvalidTile: return 'oda nem léphetsz!';
	        case ResultCode.NotSzomszedTile: return 'ez túl messze van... Szomszédos lapkára kell lépni, kivéve ha barlanglapkán vagy.';
	        case ResultCode.NoWeapon: return 'nincs fegyvered, így nem lehet mamutra vadászni! Előbb szerezz fegyvert, csak utána léphetsz mamutos lapkára, barátom.';
	        case ResultCode.MoreBabusNotAllowed: return 'ide nem lehet lépni, már foglalt. Lépés befejezésekor Barlang és Táborhely lapkák kivételével egy lapkán csak egy bábu lehet!';
	        case ResultCode.SameTile: return 'na ne, azért ne ugyanoda próbálj lépni! :)';
	            //Ok: 1
	        default: return 'ismeretlen ResultCode !';
	    }
	}
};

