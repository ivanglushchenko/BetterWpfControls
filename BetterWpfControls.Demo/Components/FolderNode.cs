using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterWpfControls.Demo.Components
{
    public partial class FolderNode : TreeListBoxItem
    {
        #region .ctors

        public FolderNode(string name, List<string> files)
        {
            Name = name;
            if (files != null)
            {
                foreach (var item in files)
                {
                    Children.Add(new FileNode() { Name = item });
                }
            }
        }

        #endregion .ctors

        #region Properties

        public string Name { get; set; }

        #endregion Properties
    }
}
