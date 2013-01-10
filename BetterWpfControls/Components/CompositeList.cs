using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BetterWpfControls.Components
{
    public class CompositeList : IList, INotifyCollectionChanged
    {
        #region .ctors

        public CompositeList()
        {
            EnableCaching = true;
        }

        #endregion .ctors

        #region Fields

        private object _syncRoot = new object();
        private List<Node> _collections = new List<Node>();
        private Dictionary<int, object> _cache = new Dictionary<int, object>();

        #endregion Fields

        #region Properties

        public string Name { get; set; }

        public bool Suspend { get; set; }

        public bool EnableCaching { get; set; }

        public IEnumerable<IList> Collections
        {
            get
            {
                return _collections.Select(c => c.Collection).ToList();
            }
        }

        #endregion Properties

        #region Methods

        public void AddCollection(IList col)
        {
            Debug.Assert(col != null);
            InsertCollection(_collections.Count, col);
        }

        public void InsertCollection(int index, IList col)
        {
            Debug.Assert(col != null);
            var node = new Node() { Collection = col, StartingPoint = Count };
            _collections.Insert(index, node);

            if (col is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)col).CollectionChanged += new NotifyCollectionChangedEventHandler(CompositeList_CollectionChanged);
            }
        }

        public void InsertCollection<T>(int index, ObservableCollection<T> col)
        {
            var node = new Node() { Collection = col, StartingPoint = Count };
            _collections.Insert(index, node);

            col.CollectionChanged += new NotifyCollectionChangedEventHandler(CompositeList_CollectionChanged);
        }

        public void InsertCollection<T>(int index, CompositeList col)
        {
            var node = new Node() { Collection = col, StartingPoint = Count };
            _collections.Insert(index, node);

            col.CollectionChanged += new NotifyCollectionChangedEventHandler(CompositeList_CollectionChanged);
        }

        public void ClearCollections()
        {
            while (_collections.Count > 0)
            {
                _collections.Remove(_collections.First());
            }
        }

        public void RemoveCollection(IList col)
        {
            var node = _collections.FirstOrDefault(c => c.Collection == col);
            if (node != null)
            {
                _collections.Remove(node);
            }
        }

        public void Reset()
        {
            if (Suspend)
            {
                return;
            }

            ResetCore(true);
        }

        private void ResetCore(bool throwReset)
        {
            if (Suspend)
            {
                return;
            }

            _cache = new Dictionary<int, object>();

            var previousEndPoint = 0;
            foreach (var item in _collections)
            {
                item.StartingPoint = previousEndPoint;
                if (item.Collection is CompositeList)
                {
                    (item.Collection as CompositeList).ResetCore(false);
                }
                previousEndPoint = item.StartingPoint + item.Collection.Count;
            }

            if (throwReset)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void CompositeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Suspend)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _cache = EnableCaching ? new Dictionary<int, object>(Math.Max(4, Count)) : new Dictionary<int, object>();

                var previousEndPoint = 0;
                foreach (var item in _collections)
                {
                    item.StartingPoint = previousEndPoint;
                    previousEndPoint = item.StartingPoint + item.Collection.Count;
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action));
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var tail = _collections.SkipWhile(c => c.Collection != sender).Skip(1);
                foreach (var item in e.NewItems)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, item));
                }
                foreach (var item in tail)
                {
                    item.StartingPoint += e.NewItems.Count;
                }
                return;
            }
            throw new NotSupportedException();
        }

        #endregion Methods

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, notifyCollectionChangedEventArgs);
            }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _collections.Clear();
        }

        public bool Contains(object value)
        {
            foreach (var item in _collections)
            {
                if (item.Collection.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(object value)
        {
            foreach (var item in _collections)
            {
                var index = item.Collection.IndexOf(value);
                if (index >= 0)
                {
                    return index + item.StartingPoint;
                }
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                if (EnableCaching)
                {
                    var item = (object)null;
                    if (_cache.TryGetValue(index, out item))
                    {
                        return item;
                    }
                }
                Node node = null;
                foreach (var item in _collections)
                {
                    if (item.StartingPoint > index)
                    {
                        break;
                    }
                    node = item;
                }
                if (node == null)
                {
                    return null;
                }
                if (EnableCaching)
                {
                    var t = node.Collection[index - node.StartingPoint];
                    _cache[index] = t;
                    return t;
                }
                else
                    return node.Collection[index - node.StartingPoint];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < Count; i++)
            {
                array.SetValue(this[i], index + i);
            }
        }

        public int Count
        {
            get
            {
                if (_collections.Count == 0)
                {
                    return 0;
                }
                var lastNode = _collections.Last();
                return lastNode.StartingPoint + lastNode.Collection.Count;
            }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new CompositeListEnumerator(this);
        }

        #endregion

        #region Internal Classes

        private class Node
        {
            public IList Collection { get; set; }
            public int StartingPoint { get; set; }
        }

        #endregion Internal Classes
    }
}
