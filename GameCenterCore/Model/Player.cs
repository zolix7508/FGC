using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;

namespace GameCenterCore.Model
{
    public class Player :IPlayer
    {
        public Guid Id { get; set; }

        public string Nick { get; set; }

        //public IUser User { get; set; }
        public int UserId { get; set; }

        public byte Szinkod { get; set; }

        public string ConnectionId  { get; set; }

        public ClientStatus ClientStatus { get; set; }
    }
}
