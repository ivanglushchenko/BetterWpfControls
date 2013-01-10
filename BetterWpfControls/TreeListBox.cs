using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BetterWpfControls
{
    public class TreeListBox : ListBox
    {
        #region .ctors

        static TreeListBox()
        {
        }

        #endregion .ctors

        #region Fields

        private BooleanToVisibilityConverter _cBoolToVis = new BooleanToVisibilityConverter();
        private TreeListBoxMarginConverter _cMargin = new TreeListBoxMarginConverter();

        #endregion Fields

        #region Methods

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListBoxItemContainer;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var depObj = new TreeListBoxItemContainer();
            BindingOperations.SetBinding(depObj, FrameworkElement.VisibilityProperty, new Binding("IsVisible") { Converter = _cBoolToVis });
            BindingOperations.SetBinding(depObj, Selector.IsSelectedProperty, new Binding("IsSelected") { Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(depObj, ListBoxItem.MarginProperty, new Binding("TreeLevel") { Converter = _cMargin });
            return depObj;
        }

        #endregion Methods

        #region Internal Classes

        private class TreeListBoxMarginConverter : IValueConverter
        {
            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (parameter == null)
                    return new Thickness((int)value * 20, 0, 0, 0);
                return new Thickness((int)value * (double)parameter, 0, 0, 0);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion Internal Classes
    }
}
