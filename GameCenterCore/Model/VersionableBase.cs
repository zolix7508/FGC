using GameCenterCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore
{
    public class VersionableBase : IVersionable
    {
        public VersionInfo Version
        {
            get {
                return new VersionInfo(this.GetType().GUID);
            }
        }
    }
}
