using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;

namespace GameCenterCore
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Nick { get; set; }
    }
}
