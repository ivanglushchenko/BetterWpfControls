using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterWpfControls.Demo.Components
{
    public partial class FileNode : TreeListBoxItem
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }
        private string _Name;
        partial void OnNameChanged();

        #endregion Properties
    }
}
