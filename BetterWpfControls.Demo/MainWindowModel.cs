using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BetterWpfControls.Components;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;

namespace BetterWpfControls.Demo
{
    public partial class MainWindowModel : INotifyPropertyChanged
    {
        #region .ctors

        public MainWindowModel()
        {
            SplitButtonCommand = new DelegateCommand<object>((o) => MessageBox.Show(string.Format("Hello from SplitButton: {0}", o)));
            SplitButtonItemCommand = new DelegateCommand<object>((o) => MessageBox.Show(string.Format("Hello from SplitButtonItem: {0}", o)));
            AddTabCommand = new DelegateCommand(AddTab);
            Items = new List<string>() { "Aaa", "Bbb", "Ccc" };
            OpenItems = new List<string>() { "Open", "Open read-only", "Open as copy", "Open in browser", "Open with transform", "Open and repair", "Show previous version" };
            Tabs1 = new ObservableCollection<object>();
            Tabs2 = new ObservableCollection<object>();
            Tabs3 = new ObservableCollection<object>();
        }

        #endregion .ctors

        #region Properties

        public ICommand SplitButtonCommand { get; set; }
        public ICommand SplitButtonItemCommand { get; set; }
        public ICommand AddTabCommand { get; set; }

        public List<string> Items { get; set; }

        public List<string> OpenItems { get; set; }
        public ObservableCollection<object> Tabs1 { get; set; }
        public ObservableCollection<object> Tabs2 { get; set; }
        public ObservableCollection<object> Tabs3 { get; set; }

        public object SelectedTab3
        {
            get { return _SelectedTab3; }
            set
            {
                if (_SelectedTab3 != value)
                {
                    _SelectedTab3 = value;
                    OnPropertyChanged("SelectedTab3");
                    OnSelectedTab3Changed();
                }
            }
        }
        private object _SelectedTab3;
        partial void OnSelectedTab3Changed();
        #endregion Properties

        #region Methods

        private void AddTab()
        {
            Tabs1.Add(new TextBlock() { Text = string.Format("CONTENT tab item {0}", Tabs1.Count + 1), Tag = string.Format("TAG tab item {0}", Tabs1.Count + 1) });
            Tabs2.Add(new TextBlock() { Text = string.Format("CONTENT tab item {0}", Tabs2.Count + 1), Tag = string.Format("TAG tab item {0}", Tabs2.Count + 1) });
            Tabs3.Add(SelectedTab3 = new TextBlock() { Text = string.Format("CONTENT tab item {0}", Tabs3.Count + 1), Tag = string.Format("TAG tab item {0}", Tabs3.Count + 1) });
        }

        #endregion Methods

        #region Internal Classes

        private class A
        {
            public string Caption { get; set; }
        }

        #endregion Internal Classes

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
            }
        }
    }
}