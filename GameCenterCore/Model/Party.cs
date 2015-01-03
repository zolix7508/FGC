using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;

namespace GameCenterCore.Model
{
    public class Party : IParty
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public PartyStatus StatusId { get; set; }
        public string Status { get { return _status; } }
        public DateTime CreatedDate { get { return _createdDate; } }

        public bool IsRunning { get { return this.StatusId == PartyStatus.Running; } }
        public IEnumerable<IPlayer> Players { get; set; }

        public DateTime? UpdatedDateUtc { get; set; }
        public string Version { get; set; }
        public string Data { get; set; }

        private PartyStatus _statusId = PartyStatus.Created;
        private string _status;
        private DateTime _createdDate;
    }
}
