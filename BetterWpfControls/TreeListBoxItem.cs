using BetterWpfControls.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BetterWpfControls
{
    public partial class TreeListBoxItem : INotifyPropertyChanged
    {
        #region .ctors

        public TreeListBoxItem()
        {
            Children = new ObservableCollection<TreeListBoxItem>();
            Children.CollectionChanged += Children_CollectionChanged;
            ChildrenProxy = new CompositeListProxy<TreeListBoxItem>(Children, e => e.List);
            List = new CompositeList();
            List.AddCollection(new TreeListBoxItem[] { this });
            List.AddCollection(ChildrenProxy);

            IsVisible = true;
            IsSelected = false;
        }

        public TreeListBoxItem(bool isExanded)
        {
            _IsExpanded = isExanded;

            Children = new ObservableCollection<TreeListBoxItem>();
            ChildrenProxy = new CompositeListProxy<TreeListBoxItem>(Children, e => e.List);
            List = new CompositeList();
            List.AddCollection(new TreeListBoxItem[] { this });

            if (isExanded)
            {
                List.AddCollection(ChildrenProxy);
            }

            IsVisible = true;
            IsSelected = false;
        }

        #endregion .ctors

        #region Properties

        public int TreeLevel
        {
            get { return _TreeLevel; }
            set
            {
                if (_TreeLevel != value)
                {
                    _TreeLevel = value;
                    OnPropertyChanged("TreeLevel");
                    OnTreeLevelChanged();
                }
            }
        }
        private int _TreeLevel;
        partial void OnTreeLevelChanged();

        public bool CanExpand
        {
            get { return _CanExpand; }
            set
            {
                if (_CanExpand != value)
                {
                    _CanExpand = value;
                    OnPropertyChanged("CanExpand");
                    OnCanExpandChanged();
                }
            }
        }
        private bool _CanExpand;
        partial void OnCanExpandChanged();

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnIsExpandedChanged();
                }
            }
        }
        private bool _IsExpanded = true;
        partial void OnIsExpandedChanged();

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }
        private bool _IsSelected;
        partial void OnIsSelectedChanged();

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                    OnIsVisibleChanged();
                }
            }
        }
        private bool _IsVisible;
        partial void OnIsVisibleChanged();

        public ObservableCollection<TreeListBoxItem> Children
        {
            get { return _Children; }
            private set
            {
                if (_Children != value)
                {
                    _Children = value;
                    OnPropertyChanged("Children");
                    OnChildrenChanged();
                }
            }
        }
        private ObservableCollection<TreeListBoxItem> _Children;
        partial void OnChildrenChanged();

        public CompositeListProxy<TreeListBoxItem> ChildrenProxy
        {
            get { return _ChildrenProxy; }
            set
            {
                if (_ChildrenProxy != value)
                {
                    _ChildrenProxy = value;
                    OnPropertyChanged("ChildrenProxy");
                    OnChildrenProxyChanged();
                }
            }
        }
        private CompositeListProxy<TreeListBoxItem> _ChildrenProxy;
        partial void OnChildrenProxyChanged();

        public CompositeList List
        {
            get { return _List; }
            set
            {
                if (_List != value)
                {
                    _List = value;
                    OnPropertyChanged("List");
                    OnListChanged();
                }
            }
        }
        private CompositeList _List;
        partial void OnListChanged();

        #endregion Properties

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
            }
        }

        #endregion INotifyPropertyChanged Implementation

        #region Methods

        partial void OnIsExpandedChanged()
        {
            if (IsExpanded)
            {
                if (List.Count == 1)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        List.AddCollection(ChildrenProxy);
                        List.Reset();
                    }));
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    List.RemoveCollection(ChildrenProxy);
                    List.Reset();
                }));
            }
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<TreeListBoxItem>())
                {
                    item.TreeLevel = TreeLevel + 1;
                }
            }
        }

        partial void OnTreeLevelChanged()
        {
            if (Children != null)
            {
                foreach (var item in Children)
                {
                    item.TreeLevel = TreeLevel + 1;
                }
            }
        }

        #endregion Methods
    }
}
