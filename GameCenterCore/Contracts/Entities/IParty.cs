using GameCenterCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public enum PartyStatus : short
    {
        Created = 1,
        Running = 2,
        Finished = 3
    }

    public enum PartyPhase : byte
    {
        InitialSetup = 1,
        Playing = 2
    }

    public interface IParty
    {
        Guid GameId { get; set; }
        IGame Game { get; set; }
        Guid Id { get; set; }
        bool IsRunning { get; }
        PartyStatus StatusId { get; set; }
        IEnumerable<IPlayer> Players { get; set; }
        DateTime? UpdatedDateUtc { get; set; }
        string Version { get; set; }
        string Data { get; set; }
    }
}
