angular.module('mvDataServiceApp', [])
.service('mvDataService', function () {
    this.getData = function () { return app.getData() };
});

var mvApp = angular.module('mvApp', ['mvDataServiceApp']);

mvApp.controller('boardController', ['$scope', 'mvDataService', function (scope, mvDataService) {
    scope.szin = graphics.getSzinClass;

    scope.exData = function (d) {
        if (!d) return;
        var n = d.szabadSzinek.indexOf(d.jatekos.Szinkod);
        if (n == -1) {
            d.szabadSzinek.push(d.jatekos.Szinkod);
            d.szabadSzinek.sort();
        }
        n = d.szabadSzinek.indexOf(0);
        if (n == -1) d.szabadSzinek = [0].concat(d.szabadSzinek);
        scope.szabadSzinek = d.szabadSzinek;
        scope.selectedSzin = d.jatekos.Szinkod; //{ kod: d.jatekos.Szinkod, cssClass: scope.szin(d.jatekos.Szinkod) };
        scope.partyPhase = d.partyPhase;
        scope.players = d.players;
        if (d.players && d.CurrentPlayerIdx >= 0) d.currentPlayer = d.players[d.CurrentPlayerIdx];
        scope.currentPlayer = d.currentPlayer;
        scope.currentPlayerLepes = d.CurrentPlayerLepes;
        scope.currentPlayerStr = function () {
            return scope.currentPlayer != null ? Localizer.nextPlayer(scope.currentPlayer.Nick, scope.currentPlayerLepes + 1) : '';
        };
        scope.connectionStatus = function (clientStatus) {
            return Localizer.connStatus(clientStatus == 1);
        }
        scope.message = d.message;
        scope.isThisClient = function (playerId) {
            return playerId == d.jatekos.Id;
        };
        scope.szinValasztas = function (szin) {
            app.szinSelected(szin);
        };
        scope.mehet = function () {
            app.szinMehet();
        };
    }
    //scope.exData( mvDataService.getData());
}]);
