using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IUser
    {
        int Id { get; set; }
        string Nick { get; set; }
        string UserName { get; set; }
    }
}
