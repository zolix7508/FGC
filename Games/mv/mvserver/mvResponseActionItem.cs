using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    internal class mvResponseActionItem : mvResponseItemBase
    {
        public override string msgType { get { return "a"; } }
        private object[] _prms;
        private ActionKind _actionKind;

        public ActionKind ActionKind { get { return _actionKind; } }

        public mvResponseActionItem(ActionKind actionKind, params object[] p)
        {
            _prms = p;
            _actionKind = actionKind;
        }

        public object[] Items { get { return _prms.ToArray(); } }
    }
}
