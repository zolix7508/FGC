using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IPlayer
    {
        Guid UID { get; }
        int UserId { get; }
    }
}
