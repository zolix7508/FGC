using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IGame : IVersionable
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
