using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterWpfControls
{
    public class ContentBox : ComboBox
    {
        #region .ctors

        static ContentBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentBox), new FrameworkPropertyMetadata(typeof(ContentBox)));
        }

        public ContentBox()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClickedHandler));

            Unloaded += OnUnloaded;
        }

        #endregion .ctors

        #region Properties

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ContentBox), new UIPropertyMetadata(null));

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentBox), new UIPropertyMetadata(null));

        public Thickness PopupPadding
        {
            get { return (Thickness)GetValue(PopupPaddingProperty); }
            set { SetValue(PopupPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupPaddingProperty =
            DependencyProperty.Register("PopupPadding", typeof(Thickness), typeof(ContentBox), new UIPropertyMetadata(new Thickness(0)));

        #endregion Properties

        #region Methods

        private void ButtonClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                IsDropDownOpen = false;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;
            RemoveHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClickedHandler));
        }

        #endregion Methods
    }
}
