using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BetterWpfControls.Components
{
    public partial class CompositeListProxy<T> : CompositeList
    {
        #region .ctors

        public CompositeListProxy(ObservableCollection<T> collection, Func<T, CompositeList> getter)
        {
            _getter = getter;
            _collection = collection;
            _collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collection_CollectionChanged);
        }

        #endregion .ctors

        #region Fields

        private Func<T, CompositeList> _getter;
        private ObservableCollection<T> _collection;

        #endregion Fields

        public Predicate<T> Filter
        {
            get { return _Filter; }
            set
            {
                if (_Filter != value)
                {
                    _Filter = value;
                    OnFilterChanged();
                }
            }
        }
        private Predicate<T> _Filter;
        partial void OnFilterChanged();

        #region Methods

        private void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<T>().Where(t => Filter == null || Filter(t)))
                {
                    AddCollection(_getter(item));
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<T>())
                {
                    RemoveCollection(_getter(item));
                }
            }
        }

        partial void OnFilterChanged()
        {
            ClearCollections();

            foreach (var item in _collection.Where(t => Filter == null || Filter(t)))
            {
                AddCollection(_getter(item));
            }

            Reset();
        }

        #endregion Methods
    }
}
