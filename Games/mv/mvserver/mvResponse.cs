using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    [Serializable]
    internal class mvResponse
    {
        private List<mvResponseItemBase> _items = new List<mvResponseItemBase>();

        public void Add(mvResponseItemBase a)
        {
            _items.Add(a);
        }

        public void AddMsgItem(params object[] prms)
        {
            var item = new mvResponseMsgItem(prms);
            _items.Add(item);
        }

        public void AddActionItem(ActionKind actionKind, params object[] prms)
        {
            var item = new mvResponseActionItem(actionKind, prms);
            _items.Add(item);
        }

        public bool StatusChanged
        {
            get
            {
                return _items.OfType<mvResponseActionItem>().Any(x => x.ActionKind != ActionKind.SelectBabu);
            }
        }

        public IEnumerable<mvResponseItemBase> GetItems()
        {
            foreach (var item in _items)
                yield return item;
        }

        public bool isEmpty
        {
            get
            {
                return _items.Count == 0;
            }
        }
    }
}
