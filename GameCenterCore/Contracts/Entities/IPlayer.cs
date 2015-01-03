using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public enum ClientStatus : byte
    {
        Unknown = 0,
        Connected = 1,
        Offline = 2
    }

    public interface IPlayer
    {
        ClientStatus ClientStatus { get; set; }
        Guid Id { get; set; }
        string Nick { get; set; }
        int UserId { get; set; }
        byte Szinkod { get; set; }
        string ConnectionId { get; set; }
        //IUser User { get; set; }
    }
}
