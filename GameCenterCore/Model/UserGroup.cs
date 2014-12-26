using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore
{
    public class UserGroup
    {
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Lazy<int> s = new Lazy<int>(delegate { return 0; });
    }
}
