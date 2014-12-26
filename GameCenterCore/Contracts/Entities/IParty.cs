using GameCenterCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    //public enum PartyStatus : int
    //{
    //    Running=1
    //}

    public interface IParty
    {
        //protected IGame Game;
        //protected List<IPlayer> Players;
        //protected PartyOptions Options { get; set; }
        //PartyStatus PartyStatus { get; }
        bool IsRunning { get; }
        IEnumerable<IPlayer> Players { get; }
    }
}
