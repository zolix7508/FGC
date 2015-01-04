using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    internal abstract class mvResponseItemBase
    {
        public abstract string msgType { get; }
    }
}
