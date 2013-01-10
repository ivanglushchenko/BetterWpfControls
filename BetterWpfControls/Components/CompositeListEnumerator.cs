using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterWpfControls.Components
{
    public class CompositeListEnumerator : IEnumerator
    {
        #region .ctors

        public CompositeListEnumerator(CompositeList list)
        {
            _list = list;
        }

        #endregion .ctors

        #region Fields

        private CompositeList _list;
        private int? index;

        #endregion Fields

        #region IEnumerator Members

        public object Current
        {
            get { return _list[index.Value]; }
        }

        public bool MoveNext()
        {
            if (index == null)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            if (_list.Count > index)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
