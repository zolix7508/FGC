using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore
{
    public class VersionInfo : IComparable<VersionInfo>
    {
        private Guid _classGuid;
        
        public Guid ClassGuid {
            get
            {
                return this._classGuid;
            }
        }

        public VersionInfo(Guid classGuid)
        {
            this._classGuid = classGuid;
        }
        



        public int CompareTo(VersionInfo other)
        {
            throw new NotImplementedException();
        }
    }
}
