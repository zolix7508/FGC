using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameCenterCore;
using GameCenterCore.Contracts;

namespace GameCenterWeb.Models
{
    public class UIPlayerModel
    {
        public Guid Id { get; set; }
        //public User User { get; set; }
        public int UserId { get; set; }
        public string Nick { get; set; }
        public bool Selected { get; set; }
    }
}