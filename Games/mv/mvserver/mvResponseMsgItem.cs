using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    internal class mvResponseMsgItem : mvResponseItemBase
    {
        public override string msgType { get { return "m"; } }
        private object[] _prms;

        public mvResponseMsgItem(params object[] p)
        {
            _prms = p;
        }

        public object[] Message { get { return _prms.ToArray(); } }
    }
}
